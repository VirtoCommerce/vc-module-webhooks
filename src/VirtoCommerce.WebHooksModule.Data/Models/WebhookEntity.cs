using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookEntity : AuditableEntity
    {
        public WebHookEntity()
        {
            Events = new NullCollection<WebHookEventEntity>();
        }

        [StringLength(1024)]
        public string Name { get; set; }
        [StringLength(2083)]
        public string Url { get; set; }
        [StringLength(128)]
        public string ContentType { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllEvents { get; set; }
        public virtual ObservableCollection<WebHookEventEntity> Events { get; set; }
        public virtual Webhook ToModel(Webhook webHook)
        {
            if (webHook == null)
                throw new ArgumentNullException(nameof(webHook));

            webHook.Id = Id;
            webHook.CreatedBy = CreatedBy;
            webHook.CreatedDate = CreatedDate;
            webHook.ModifiedBy = ModifiedBy;
            webHook.ModifiedDate = ModifiedDate;
            webHook.Name = Name;
            webHook.Url = Url;
            webHook.ContentType = ContentType;
            webHook.IsActive = IsActive;
            webHook.IsAllEvents = IsAllEvents;

            webHook.Events = Events.Select(x => x.ToModel(AbstractTypeFactory<WebhookEvent>.TryCreateInstance())).ToArray();

            return webHook;
        }

        public virtual WebHookEntity FromModel(Webhook webHook, PrimaryKeyResolvingMap pkMap)
        {
            if (webHook == null)
                throw new ArgumentNullException(nameof(webHook));

            Id = webHook.Id;
            CreatedBy = webHook.CreatedBy;
            CreatedDate = webHook.CreatedDate;
            ModifiedBy = webHook.ModifiedBy;
            ModifiedDate = webHook.ModifiedDate;
            Name = webHook.Name;
            Url = webHook.Url;
            ContentType = webHook.ContentType;
            IsActive = webHook.IsActive;
            IsAllEvents = webHook.IsAllEvents;

            if (webHook.Events != null)
            {
                Events = new ObservableCollection<WebHookEventEntity>(webHook.Events.Select(x => AbstractTypeFactory<WebHookEventEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }
            pkMap.AddPair(webHook, this);

            return this;
        }

        public virtual void Patch(WebHookEntity target)
        {
            target.Name = Name;
            target.Url = Url;
            target.ContentType = ContentType;
            target.IsActive = IsActive;
            target.IsAllEvents = IsAllEvents;

            if (!Events.IsNullCollection())
            {
                var eventComparer = AnonymousComparer.Create((WebHookEventEntity x) => $"{x.EventId}");
                Events.Patch(target.Events, eventComparer, (sourceEvent, targetEvent) => sourceEvent.Patch(targetEvent));
            }
        }
    }
}
