using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebhookEvent : AuditableEntity
	{
		public string WebhookId { get; set; }
		public string EventId { get; set; }
	}
}
