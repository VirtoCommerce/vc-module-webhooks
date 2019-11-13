using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
    public interface IWebhookSearchService
    {
        GenericSearchResult<Webhook> Search(WebhookSearchCriteria searchCriteria);
        GenericSearchResult<WebhookFeed> SearchFeed(WebhookSearchCriteria searchCriteria);
    }
}