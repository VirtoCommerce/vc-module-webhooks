using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookRequest
    {
        public string EventId { get; set; }
        public string EventObject { get; set; }
        public ICollection<Webhook> WebHooks { get; set; }
    }
}
