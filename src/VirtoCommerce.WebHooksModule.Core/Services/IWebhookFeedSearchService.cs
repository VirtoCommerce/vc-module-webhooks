using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookFeedSearchService
	{
		Task<WebHookFeedSearchResult> SearchAsync(WebHookFeedSearchCriteria searchCriteria);
	}
}
