using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebhooksModule.Data.Services;
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
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString("VirtoCommerce.Catalog") ?? configuration.GetConnectionString("VirtoCommerce"));
            });
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
            var webHookManager = appBuilder.ApplicationServices.GetService<IWebHookManager>();
            webHookManager.SubscribeToAllEvents();

            //Force migrations
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                var catalogDbContext = serviceScope.ServiceProvider.GetRequiredService<WebhookDbContext>();
                catalogDbContext.Database.MigrateIfNotApplied(MigrationName.GetUpdateV2MigrationName(ModuleInfo.Id));
                catalogDbContext.Database.EnsureCreated();
                catalogDbContext.Database.Migrate();
            }
        }

        public void Uninstall()
        {
            // Method intentionally left empty.
        }
    }
}
