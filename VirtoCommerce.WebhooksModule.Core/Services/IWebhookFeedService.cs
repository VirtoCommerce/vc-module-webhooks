using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebhookFeedService
	{
		WebhookFeedEntry[] GetByIds(string[] ids);
		void DeleteByIds(string[] ids);
		void SaveChanges(WebhookFeedEntry[] webhookLogEntries);
	}
}
