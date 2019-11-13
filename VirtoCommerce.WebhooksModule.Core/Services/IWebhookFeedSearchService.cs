using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebhookFeedSearchService
	{
		WebhookFeedSearchResult Search(WebhookFeedSearchCriteria searchCriteria);
	}
}
