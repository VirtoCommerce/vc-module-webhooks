using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.Platform.Data.Infrastructure.Interceptors;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public class WebHookRepository : EFRepositoryBase, IWebHookRepository
    {
        public WebHookRepository()
        {
        }

        public WebHookRepository(string nameOrConnectionString, params IInterceptor[] interceptors) : base(nameOrConnectionString, null, interceptors)
        {
            Database.SetInitializer<WebHookRepository>(null);
        }

        public WebHookRepository(DbConnection existingConnection, IInterceptor[] interceptors = null) : base(existingConnection, null, interceptors)
        {
        }

        public IQueryable<WebHookEntity> WebHooks => GetAsQueryable<WebHookEntity>();
        public IQueryable<WebHookEventEntity> WebHookEvents => GetAsQueryable<WebHookEventEntity>();
        public IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries => GetAsQueryable<WebHookFeedEntryEntity>();
        public WebHookEntity[] GetWebHooksByIds(string[] ids)
        {
            return WebHooks
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .ToArray();
        }

        public void DeleteWebHooksByIds(string[] ids)
        {
            var webHooks = GetWebHooksByIds(ids);
            foreach (var webHook in webHooks)
            {
                Remove(webHook);
            }
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebHookEntity>().ToTable("WebHook").HasKey(x => x.Id).Property(x => x.Id);

            modelBuilder.Entity<WebHookEventEntity>().ToTable("WebHookEvent").HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<WebHookEventEntity>().HasOptional(m => m.WebHook).WithMany(x => x.Events).HasForeignKey(x => x.WebHookId).WillCascadeOnDelete(false);

            modelBuilder.Entity<WebHookFeedEntryEntity>().ToTable("WebHookFeedEntry").HasKey(x => x.Id).Property(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}