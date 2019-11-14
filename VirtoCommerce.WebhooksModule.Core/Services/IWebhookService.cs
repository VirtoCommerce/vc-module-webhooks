using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookService
	{
		WebHook[] GetByIds(string[] ids);
		void DeleteByIds(string[] ids);
		void SaveChanges(WebHook[] webhooks);
	}
}
