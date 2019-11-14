using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookSearchService
	{
		WebHookSearchResult Search(WebHookSearchCriteria searchCriteria);
	}
}