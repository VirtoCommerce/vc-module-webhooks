using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
    public interface IWebhookSearchService
    {
        GenericSearchResult<WebWebhook> Search(WebhookSearchCriteria searchCriteria);
        GenericSearchResult<WebWebhookFeed> SearchFeed(WebhookSearchCriteria searchCriteria);
    }
}