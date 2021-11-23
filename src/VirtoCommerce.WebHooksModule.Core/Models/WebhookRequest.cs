using System;
using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookRequest
    {
        public string EventId { get; set; }
        [Obsolete("EventObject is obsolete and shouldn't be used. There is a WebhooksPayload property instead.")]
        public string EventObject { get; set; }
        public Dictionary<string, string> WebhooksPayload { get; set; }
        public ICollection<Webhook> WebHooks { get; set; }
    }
}
