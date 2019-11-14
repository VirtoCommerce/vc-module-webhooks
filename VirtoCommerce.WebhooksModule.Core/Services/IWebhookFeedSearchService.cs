using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebHookFeedSearchService
	{
		WebHookFeedSearchResult Search(WebHookFeedSearchCriteria searchCriteria);
	}
}
