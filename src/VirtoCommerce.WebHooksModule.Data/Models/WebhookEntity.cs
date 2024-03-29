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

        /// <summary>
        /// Authentication type
        /// </summary>
        public AuthenticationType AuthType { get; set; }

        /// <summary>
        /// Basic authentication username
        /// </summary>
        public string BasicUsername { get; set; }

        /// <summary>
        /// Basic authentication password
        /// </summary>
        public string BasicPassword { get; set; }

        /// <summary>
        /// Bearer token for BearerToken 
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// Name of Custom Http Header.
        /// </summary>
        public string CustomHttpHeaderName { get; set; }

        /// <summary>
        /// Value of Custom Http Header.
        /// </summary>
        public string CustomHttpHeaderValue { get; set; }

        [Obsolete("Use only one event for subscribing. This property would be removed in the future releases.")]
        public bool IsAllEvents { get; set; }
        public virtual ObservableCollection<WebHookEventEntity> Events { get; set; }
        public virtual ObservableCollection<WebHookPayloadEntity> Payloads { get; set; }

        public virtual Webhook ToModel(Webhook webHook)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

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

            webHook.AuthType = AuthType;
            webHook.BasicUsername = BasicUsername;
            webHook.BasicPassword = BasicPassword;
            webHook.BearerToken = BearerToken;
            webHook.CustomHttpHeaderName = CustomHttpHeaderName;
            webHook.CustomHttpHeaderValue = CustomHttpHeaderValue;

            webHook.Events = Events.Select(x => x.ToModel(AbstractTypeFactory<WebhookEvent>.TryCreateInstance())).ToArray();

            webHook.Payloads = Payloads.Select(x => x.ToModel(AbstractTypeFactory<WebHookPayload>.TryCreateInstance())).ToArray();

            return webHook;
        }

        public virtual WebHookEntity FromModel(Webhook webHook, PrimaryKeyResolvingMap pkMap)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

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

            AuthType = webHook.AuthType;
            BasicUsername = webHook.BasicUsername;
            BasicPassword = webHook.BasicPassword;
            BearerToken = webHook.BearerToken;
            CustomHttpHeaderName = webHook.CustomHttpHeaderName;
            CustomHttpHeaderValue = webHook.CustomHttpHeaderValue;


            if (webHook.Events != null)
            {
                Events = new ObservableCollection<WebHookEventEntity>(webHook.Events.Select(x => AbstractTypeFactory<WebHookEventEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            if (webHook.Payloads != null)
            {
                Payloads = new ObservableCollection<WebHookPayloadEntity>(webHook.Payloads.Select(x => AbstractTypeFactory<WebHookPayloadEntity>.TryCreateInstance().FromModel(x, pkMap)));
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

            target.AuthType = AuthType;
            target.BasicUsername = BasicUsername;
            target.BasicPassword = BasicPassword;
            target.BearerToken = BearerToken;
            target.CustomHttpHeaderName = CustomHttpHeaderName;
            target.CustomHttpHeaderValue = CustomHttpHeaderValue;

            if (!Events.IsNullCollection())
            {
                var eventComparer = AnonymousComparer.Create((WebHookEventEntity x) => $"{x.EventId}");
                Events.Patch(target.Events, eventComparer, (sourceEvent, targetEvent) => sourceEvent.Patch(targetEvent));
            }

            if (!Payloads.IsNullCollection())
            {
                var payloadComparer = AnonymousComparer.Create((WebHookPayloadEntity x) => $"{x.EventPropertyName}");
                Payloads.Patch(target.Payloads, payloadComparer, (sourcePayload, targetPayload) => sourcePayload.Patch(targetPayload));
            }
        }
    }
}
