using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebhooksModule.Data.Services;
using VirtoCommerce.WebHooksModule.Core;
using VirtoCommerce.WebHooksModule.Core.Services;
using VirtoCommerce.WebHooksModule.Data.Services;

namespace VirtoCommerce.WebHooksModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IWebHookRepository, WebHookRepository>();
            serviceCollection.AddDbContext<WebhookDbContext>((provider, options) =>
                options.UseSqlServer(provider.GetRequiredService<IConfiguration>().GetConnectionString("VirtoCommerce")));
            serviceCollection.AddTransient<Func<IWebHookRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IWebHookRepository>());

            serviceCollection.AddTransient<IWebHookService, WebHookService>();
            serviceCollection.AddTransient<IWebHookSearchService, WebHookSearchService>();

            serviceCollection.AddTransient<IWebHookFeedService, WebHookFeedService>();
            serviceCollection.AddTransient<IWebHookFeedSearchService, WebHookFeedService>();
            serviceCollection.AddTransient<IWebHookFeedReader, WebHookFeedService>();

            serviceCollection.AddSingleton<IWebHookLogger>(provider =>
                new WebHookLogger(provider.GetService<IWebHookFeedService>(), provider.GetService<IWebHookFeedSearchService>()));

            serviceCollection.AddSingleton<IRegisteredEventStore,RegisteredEventStore>();
            serviceCollection.AddTransient<IWebHookSender, RetriableWebHookSender>();
            serviceCollection.AddTransient<IWebHookManager, WebHookManager>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var settingsRegistrar = appBuilder.ApplicationServices.GetRequiredService<ISettingsRegistrar>();
            settingsRegistrar.RegisterSettings(ModuleConstants.Settings.General.AllSettings, ModuleInfo.Id);

            //Register module permissions
            var permissionsProvider = appBuilder.ApplicationServices.GetRequiredService<IPermissionsRegistrar>();
            permissionsProvider.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions.Select(x =>
                new Permission() { GroupName = "Webhooks", Name = x }).ToArray());

            var webHookManager = appBuilder.ApplicationServices.GetService<IWebHookManager>();
            webHookManager.SubscribeToAllEvents();

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<WebhookDbContext>();
                dbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}
