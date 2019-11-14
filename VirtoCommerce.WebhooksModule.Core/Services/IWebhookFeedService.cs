using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebHookFeedService
	{
		WebHookFeedEntry[] GetByIds(string[] ids);
		void DeleteByIds(string[] ids);
		void SaveChanges(WebHookFeedEntry[] webhookLogEntries);
	}
}
