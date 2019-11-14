using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookFeedService
	{
		WebHookFeedEntry[] GetByIds(string[] ids);
		void DeleteByIds(string[] ids);
		void SaveChanges(WebHookFeedEntry[] webhookLogEntries);
	}
}
