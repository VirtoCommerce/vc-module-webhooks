using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebHookSearchService
	{
		WebHookSearchResult Search(WebHookSearchCriteria searchCriteria);
	}
}