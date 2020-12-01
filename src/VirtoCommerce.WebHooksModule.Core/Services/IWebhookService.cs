using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookService
	{
		Task<WebHook[]> GetByIdsAsync(string[] ids, string responseGroup = null);
        Task DeleteByIdsAsync(string[] ids);
        Task SaveChangesAsync(WebHook[] webHooks);
	}
}
