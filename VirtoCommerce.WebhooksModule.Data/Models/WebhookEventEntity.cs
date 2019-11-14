using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookEventEntity : AuditableEntity
    {
        [StringLength(128)]
        public string EventId { get; set; }

        #region Navigation Properties
        [StringLength(128)]
        public string WebhookId { get; set; }
        public virtual WebHookEntity WebHook { get; set; }
        #endregion
    }
}