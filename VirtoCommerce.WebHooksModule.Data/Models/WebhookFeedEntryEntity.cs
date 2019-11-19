using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookFeedEntryEntity : AuditableEntity
    {
        [StringLength(128)]
        [Index]
        [Index("IX_WebHookIdAndStatus", 1)]
        public string WebHookId { get; set; }
        [StringLength(128)]
        public string EventId { get; set; }
        public int AttemptCount { get; set; }
        [Index("IX_WebHookIdAndStatus", 2)]
        public int Status { get; set; }
        [StringLength(1024)]
        public string Error { get; set; }
        // max avaliable size for headers is 16384 
        [MaxLength]
        public string RequestHeaders { get; set; }
        [MaxLength]
        public string RequestBody { get; set; }
        [MaxLength]
        public string ResponseHeaders { get; set; }
        [MaxLength]
        public string ResponseBody { get; set; }

        public virtual WebHookFeedEntry ToModel(WebHookFeedEntry webHookFeedEntry)
        {
            if (webHookFeedEntry == null)
                throw new ArgumentNullException(nameof(webHookFeedEntry));

            webHookFeedEntry.Id = this.Id;
            webHookFeedEntry.CreatedBy = this.CreatedBy;
            webHookFeedEntry.CreatedDate = this.CreatedDate;
            webHookFeedEntry.ModifiedBy = this.ModifiedBy;
            webHookFeedEntry.ModifiedDate = this.ModifiedDate;
            webHookFeedEntry.WebHookId = this.WebHookId;
            webHookFeedEntry.EventId = this.EventId;
            webHookFeedEntry.AttemptCount = this.AttemptCount;
            webHookFeedEntry.Status = this.Status;
            webHookFeedEntry.Error = this.Error;
            webHookFeedEntry.RequestHeaders = this.RequestHeaders;
            webHookFeedEntry.RequestBody = this.RequestBody;
            webHookFeedEntry.ResponseHeaders = this.ResponseHeaders;
            webHookFeedEntry.ResponseBody = this.ResponseBody;

            return webHookFeedEntry;
        }

        public virtual WebHookFeedEntryEntity FromModel(WebHookFeedEntry webHookFeedEntry, PrimaryKeyResolvingMap pkMap)
        {
            if (webHookFeedEntry == null)
                throw new ArgumentNullException(nameof(webHookFeedEntry));

            this.Id = webHookFeedEntry.Id;
            this.CreatedBy = webHookFeedEntry.CreatedBy;
            this.CreatedDate = webHookFeedEntry.CreatedDate;
            this.ModifiedBy = webHookFeedEntry.ModifiedBy;
            this.ModifiedDate = webHookFeedEntry.ModifiedDate;
            this.WebHookId = webHookFeedEntry.WebHookId;
            this.EventId = webHookFeedEntry.EventId;
            this.AttemptCount = webHookFeedEntry.AttemptCount;
            this.Status = webHookFeedEntry.Status;
            this.Error = webHookFeedEntry.Error;
            this.RequestHeaders = webHookFeedEntry.RequestHeaders;
            this.RequestBody = webHookFeedEntry.RequestBody;
            this.ResponseHeaders = webHookFeedEntry.ResponseHeaders;
            this.ResponseBody = webHookFeedEntry.ResponseBody;

            pkMap.AddPair(webHookFeedEntry, this);

            return this;
        }

        public virtual void Patch(WebHookFeedEntry target)
        {
            target.WebHookId = this.WebHookId;
            target.EventId = this.EventId;
            target.AttemptCount = this.AttemptCount;
            target.Status = this.Status;
            target.Error = this.Error;
            target.RequestHeaders = this.RequestHeaders;
            target.RequestBody = this.RequestBody;
            target.ResponseHeaders = this.ResponseHeaders;
            target.ResponseBody = this.ResponseBody;
        }
    }
}
