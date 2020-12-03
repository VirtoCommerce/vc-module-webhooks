using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookRequest
    {
        public string EventId { get; set; }
        public object EventObject { get; set; }
        public ICollection<Webhook> WebHooks { get; set; }
    }
}
