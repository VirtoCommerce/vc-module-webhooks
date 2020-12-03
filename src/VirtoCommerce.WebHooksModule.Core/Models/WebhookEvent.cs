using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookEvent : AuditableEntity
    {
        public string WebHookId { get; set; }
        public string EventId { get; set; }
    }
}
