using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookEventEntity : AuditableEntity
    {
        [StringLength(128)]
        public string EventId { get; set; }

        #region Navigation Properties
        [StringLength(128)]
        public string WebHookId { get; set; }
        public virtual WebHookEntity WebHook { get; set; }
        #endregion

        public virtual WebhookEvent ToModel(WebhookEvent webHookEvent)
        {
            if (webHookEvent == null)
                throw new ArgumentNullException(nameof(webHookEvent));

            webHookEvent.Id = Id;
            webHookEvent.CreatedBy = CreatedBy;
            webHookEvent.CreatedDate = CreatedDate;
            webHookEvent.ModifiedBy = ModifiedBy;
            webHookEvent.ModifiedDate = ModifiedDate;
            webHookEvent.EventId = EventId;
            webHookEvent.WebHookId = WebHookId;

            return webHookEvent;
        }

        public virtual WebHookEventEntity FromModel(WebhookEvent webHookEvent, PrimaryKeyResolvingMap pkMap)
        {
            if (webHookEvent == null)
                throw new ArgumentNullException(nameof(webHookEvent));

            Id = webHookEvent.Id;
            CreatedBy = webHookEvent.CreatedBy;
            CreatedDate = webHookEvent.CreatedDate;
            ModifiedBy = webHookEvent.ModifiedBy;
            ModifiedDate = webHookEvent.ModifiedDate;
            EventId = webHookEvent.EventId;
            WebHookId = webHookEvent.WebHookId;

            pkMap.AddPair(webHookEvent, this);

            return this;
        }

        public virtual void Patch(WebHookEventEntity target)
        {
            target.EventId = EventId;
        }
    }
}
