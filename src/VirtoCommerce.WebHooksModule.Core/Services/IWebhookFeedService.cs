using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookFeedService
	{
		Task<WebhookFeedEntry[]> GetByIdsAsync(string[] ids);
		Task DeleteByIdsAsync(string[] ids);
		Task SaveChangesAsync(WebhookFeedEntry[] webhookLogEntries);
	}
}
