using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebHookService
	{
		WebHook[] GetByIds(string[] ids);
		void DeleteByIds(string[] ids);
		void SaveChanges(WebHook[] webhooks);
	}
}
