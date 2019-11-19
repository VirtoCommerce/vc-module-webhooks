using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookFeedSearchService
	{
		WebHookFeedSearchResult Search(WebHookFeedSearchCriteria searchCriteria);
	}
}
