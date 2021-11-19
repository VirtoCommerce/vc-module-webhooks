using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebHookPayload : AuditableEntity
    {
        public string EventPropertyName { get; set; }
        public string WebHookId { get; set; }
    }
}
