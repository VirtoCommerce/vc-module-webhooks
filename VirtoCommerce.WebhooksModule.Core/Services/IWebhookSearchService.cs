using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebhookSearchService
	{
		WebhookSearchResult Search(WebhookSearchCriteria searchCriteria);
	}
}