using System.Threading.Tasks;
using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
    public interface IWebHookService
	{
        Task<Webhook[]> GetByIdsAsync(string[] ids, string responseGroup = null);
        Task DeleteByIdsAsync(string[] ids);
        Task SaveChangesAsync(Webhook[] webHooks);
	}
}
