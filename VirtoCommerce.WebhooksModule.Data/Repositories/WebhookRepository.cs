using System.Data.Entity;
using System.Linq;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public class WebHookRepository : EFRepositoryBase, IWebHookRepository
    {
        public IQueryable<WebHookEntity> WebHooks => GetAsQueryable<WebHookEntity>();
        public IQueryable<WebHookEventEntity> WebHookEvents => GetAsQueryable<WebHookEventEntity>();
        public IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries => GetAsQueryable<WebHookFeedEntryEntity>();
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebHookEntity>().ToTable("WebHook").HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<WebHookEventEntity>().ToTable("WebHookEvent").HasKey(x => x.Id).Property(x => x.Id);
            modelBuilder.Entity<WebHookFeedEntryEntity>().ToTable("WebHookFeedEntry").HasKey(x => x.Id).Property(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}