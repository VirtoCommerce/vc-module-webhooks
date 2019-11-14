using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebHooksModule.Core.Models;

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

        public virtual WebHookEvent ToModel(WebHookEvent webHookEvent)
        {
            if (webHookEvent == null)
                throw new ArgumentNullException(nameof(webHookEvent));

            webHookEvent.Id = this.Id;
            webHookEvent.CreatedBy = this.CreatedBy;
            webHookEvent.CreatedDate = this.CreatedDate;
            webHookEvent.ModifiedBy = this.ModifiedBy;
            webHookEvent.ModifiedDate = this.ModifiedDate;
            webHookEvent.EventId = this.EventId;
            webHookEvent.WebHookId = this.WebHookId;

            return webHookEvent;
        }

        public virtual WebHookEventEntity FromModel(WebHookEvent webHookEvent, PrimaryKeyResolvingMap pkMap)
        {
            if (webHookEvent == null)
                throw new ArgumentNullException(nameof(webHookEvent));

            this.Id = webHookEvent.Id;
            this.CreatedBy = webHookEvent.CreatedBy;
            this.CreatedDate = webHookEvent.CreatedDate;
            this.ModifiedBy = webHookEvent.ModifiedBy;
            this.ModifiedDate = webHookEvent.ModifiedDate;

            pkMap.AddPair(webHookEvent, this);

            return this;
        }

        public virtual void Patch(WebHookEventEntity target)
        {
            target.EventId = this.EventId;
            target.WebHookId = this.WebHookId;
        }
    }
}