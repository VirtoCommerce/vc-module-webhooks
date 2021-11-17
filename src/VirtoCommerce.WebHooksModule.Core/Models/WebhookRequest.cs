using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookRequest
    {
        public string EventId { get; set; }
        public string EventObject { get; set; }
        public DomainEvent DomainEventObject { get; set; }
        public ICollection<Webhook> WebHooks { get; set; }
    }
}
