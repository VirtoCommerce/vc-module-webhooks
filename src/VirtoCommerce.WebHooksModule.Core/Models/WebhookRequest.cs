using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookRequest
    {
        public string EventId { get; set; }
        public Dictionary<string, string> WebhooksPayload { get; set; }
        public ICollection<Webhook> WebHooks { get; set; }
    }
}
