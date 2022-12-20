using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public class WebHookRepository : DbContextRepositoryBase<WebhookDbContext>, IWebHookRepository
    {
        public WebHookRepository(WebhookDbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<WebHookEntity> WebHooks => DbContext.Set<WebHookEntity>();
        public IQueryable<WebHookEventEntity> WebHookEvents => DbContext.Set<WebHookEventEntity>();
        public IQueryable<WebHookPayloadEntity> WebHookPayloads => DbContext.Set<WebHookPayloadEntity>();
        public IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries => DbContext.Set<WebHookFeedEntryEntity>();

        public Task<WebHookEntity[]> GetWebHooksByIdsAsync(string[] ids) => WebHooks
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Events)
                .Include(x => x.Payloads)
                .ToArrayAsync();

        public async Task DeleteWebHooksByIdsAsync(string[] ids)
        {
            var webHooks = await GetWebHooksByIdsAsync(ids);
            foreach (var webHook in webHooks)
            {
                Remove(webHook);
            }
        }

        public Task<WebHookFeedEntryEntity[]> GetWebHookFeedEntriesByIdsAsync(string[] ids) => WebHookFeedEntries
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();

        public async Task DeleteWebHookFeedEntriesByIdsAsync(string[] ids)
        {
            var models = await GetWebHookFeedEntriesByIdsAsync(ids);

            foreach (var model in models)
            {
                Remove(model);
            }

        }

        public Task UpdateAttemptCountsAsync(WebHookFeedEntryEntity[] webHookFeedEntries)
        {
            if (!webHookFeedEntries.IsNullOrEmpty())
            {
                foreach (var model in webHookFeedEntries)
                {
                    Update(model);
                }
            }

            return Task.CompletedTask;
        }
    }
}
