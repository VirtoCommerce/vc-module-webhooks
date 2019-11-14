using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebhooksModule.Data.Models;

namespace VirtoCommerce.WebhooksModule.Data.Repositories
{
    public interface IWebHookRepository : IRepository
    {
        IQueryable<WebHookEntity> WebHooks { get; }
        IQueryable<WebHookEventEntity> WebHookEvents { get; }
        IQueryable<WebHookFeedEntryEntity> WebHookFeedEntries { get; }
    }
}