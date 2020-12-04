using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebhookFeedEntry : AuditableEntity
    {
        public string WebHookId { get; set; }
        public string EventId { get; set; }
        public int AttemptCount { get; set; }
        public int Status { get; set; }
        public int RecordType { get; set; }
        public string Error { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestBody { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }
    }
}
