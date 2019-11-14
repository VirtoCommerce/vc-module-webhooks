﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebHooksModule.Core.Models;

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
        public long RaisedEventCount { get; set; }
        public virtual ObservableCollection<WebHookEventEntity> Events { get; set; }
        public virtual WebHook ToModel(WebHook webHook)
        {
            if (webHook == null)
                throw new ArgumentNullException(nameof(webHook));

            webHook.Id = this.Id;
            webHook.CreatedBy = this.CreatedBy;
            webHook.CreatedDate = this.CreatedDate;
            webHook.ModifiedBy = this.ModifiedBy;
            webHook.ModifiedDate = this.ModifiedDate;
            webHook.Name = this.Name;
            webHook.Url = this.Url;
            webHook.ContentType = this.ContentType;
            webHook.IsActive = this.IsActive;
            webHook.IsAllEvents = this.IsAllEvents;
            webHook.RaisedEventCount = this.RaisedEventCount;

            webHook.Events = this.Events.Select(x => x.ToModel(new WebHookEvent())).ToArray();

            return webHook;
        }

        public virtual WebHookEntity FromModel(WebHook webHook, PrimaryKeyResolvingMap pkMap)
        {
            if (webHook == null)
                throw new ArgumentNullException(nameof(webHook));

            this.Id = webHook.Id;
            this.CreatedBy = webHook.CreatedBy;
            this.CreatedDate = webHook.CreatedDate;
            this.ModifiedBy = webHook.ModifiedBy;
            this.ModifiedDate = webHook.ModifiedDate;
            this.Name = webHook.Name;
            this.Url = webHook.Url;
            this.ContentType = webHook.ContentType;
            this.IsActive = webHook.IsActive;
            this.IsAllEvents = webHook.IsAllEvents;
            this.RaisedEventCount = webHook.RaisedEventCount;

            pkMap.AddPair(webHook, this);

            return this;
        }

        public virtual void Patch(WebHookEntity target)
        {
            target.Name = this.Name;
            target.Url = this.Url;
            target.ContentType = this.ContentType;
            target.IsActive = this.IsActive;
            target.IsAllEvents = this.IsAllEvents;
            target.RaisedEventCount = this.RaisedEventCount;

            if (!Events.IsNullCollection())
            {
                Events.Patch(target.Events, (sourceEvent, targetEvent) => sourceEvent.Patch(targetEvent));
            }
        }
    }
}