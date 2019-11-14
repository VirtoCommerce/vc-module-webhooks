using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class WebHookEvent : AuditableEntity
	{
		public string WebHookId { get; set; }
		public string EventId { get; set; }
	}
}
