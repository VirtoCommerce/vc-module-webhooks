using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookFeedEntryEntity : AuditableEntity
    {
        [StringLength(128)]
        public string WebhookId { get; set; }
        [StringLength(128)]
        public string EventId { get; set; }
        public int AttemptCount { get; set; }
        public int Status { get; set; }
        [StringLength(1024)]
        public string Error { get; set; }
        [StringLength(16384)]
        public string RequestHeaders { get; set; }
        [MaxLength]
        public string RequestBody { get; set; }
        [StringLength(16384)]
        public string ResponseHeaders { get; set; }
        [MaxLength]
        public string ResponseBody { get; set; }
    }
}