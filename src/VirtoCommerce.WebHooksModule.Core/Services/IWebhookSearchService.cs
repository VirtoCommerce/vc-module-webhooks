using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookSearchService
	{
		Task<WebHookSearchResult> SearchAsync(WebHookSearchCriteria searchCriteria);
	}
}
