using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public interface IWebHookRepository : IRepository
    {
        IQueryable<WebHookEntity> WebHooks { get; }
        IQueryable<WebHookEventEntity> WebHookEvents { get; }
        IQueryable<WebHookPayloadEntity> WebHookPayloads { get; }

        IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries { get; }

        Task<WebHookEntity[]> GetWebHooksByIdsAsync(string[] ids);
        Task DeleteWebHooksByIdsAsync(string[] ids);

        Task<WebHookFeedEntryEntity[]> GetWebHookFeedEntriesByIdsAsync(string[] ids);
        Task DeleteWebHookFeedEntriesByIdsAsync(string[] ids);
        Task UpdateAttemptCountsAsync(WebHookFeedEntryEntity[] webHookFeedEntries);
    }
}
