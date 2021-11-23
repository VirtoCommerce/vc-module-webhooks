using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    /// <summary>
    /// This entity using for storing webhook payload properties collection.
    /// </summary>
    public class WebHookPayloadEntity : AuditableEntity
    {
        [StringLength(128)]
        public string EventPropertyName { get; set; }

        [StringLength(128)]
        public string WebHookId { get; set; }
        public virtual WebHookEntity WebHook { get; set; }

        public virtual WebHookPayload ToModel(WebHookPayload payload)
        {
            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            payload.Id = Id;
            payload.CreatedBy = CreatedBy;
            payload.CreatedDate = CreatedDate;
            payload.ModifiedBy = ModifiedBy;
            payload.ModifiedDate = ModifiedDate;

            payload.EventPropertyName = EventPropertyName;
            payload.WebHookId = WebHookId;

            return payload;
        }

        public virtual WebHookPayloadEntity FromModel(WebHookPayload payload, PrimaryKeyResolvingMap pkMap)
        {
            if (payload is null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            Id = payload.Id;
            CreatedBy = payload.CreatedBy;
            CreatedDate = payload.CreatedDate;
            ModifiedBy = payload.ModifiedBy;
            ModifiedDate = payload.ModifiedDate;

            WebHookId = payload.WebHookId;
            EventPropertyName = payload.EventPropertyName;

            pkMap.AddPair(payload, this);

            return this;
        }

        public virtual void Patch(WebHookPayloadEntity target)
        {
            target.EventPropertyName = EventPropertyName;
        }
    }
}
