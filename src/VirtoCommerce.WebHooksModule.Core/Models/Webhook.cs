using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class Webhook : AuditableEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllEvents { get; set; }
        public long SuccessCount { get; set; }
        public long ErrorCount { get; set; }
        public WebhookEvent[] Events { get; set; }
        public WebhookHttpParams RequestParams { get; set; }
        public IReadOnlyCollection<string> EventPayloadProperties { get; set; }
    }
}
