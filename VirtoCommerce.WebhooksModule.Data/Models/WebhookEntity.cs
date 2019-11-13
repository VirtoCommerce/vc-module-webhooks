using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebhookEntity : AuditableEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllEvents { get; set; }
        public long RaisedEventCount { get; set; }
        public WebhookEvent[] Events { get; set; }

    }
}