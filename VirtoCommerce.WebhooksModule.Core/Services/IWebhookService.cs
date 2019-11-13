using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Core.Services
{
	public interface IWebhookService
	{
		Webhook[] GetByIds(string[] ids);
		void DeleteByIds(string[] ids);
		void SaveChanges(Webhook[] webhooks);
	}
}
