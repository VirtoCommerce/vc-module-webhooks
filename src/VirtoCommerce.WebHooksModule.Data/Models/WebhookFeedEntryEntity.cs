using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebhooksModule.Data.Models
{
    public class WebHookFeedEntryEntity : AuditableEntity
    {
        [StringLength(128)]
        public string WebHookId { get; set; }
        [StringLength(128)]
        public string EventId { get; set; }
        public int AttemptCount { get; set; }
        public int RecordType { get; set; }
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

        public virtual WebhookFeedEntry ToModel(WebhookFeedEntry webHookFeedEntry)
        {
            if (webHookFeedEntry == null)
            {
                throw new ArgumentNullException(nameof(webHookFeedEntry));
            }

            webHookFeedEntry.Id = Id;
            webHookFeedEntry.CreatedBy = CreatedBy;
            webHookFeedEntry.CreatedDate = CreatedDate;
            webHookFeedEntry.ModifiedBy = ModifiedBy;
            webHookFeedEntry.ModifiedDate = ModifiedDate;
            webHookFeedEntry.WebHookId = WebHookId;
            webHookFeedEntry.EventId = EventId;
            webHookFeedEntry.AttemptCount = AttemptCount;
            webHookFeedEntry.RecordType = RecordType;
            webHookFeedEntry.Status = Status;
            webHookFeedEntry.Error = Error;
            webHookFeedEntry.RequestHeaders = RequestHeaders;
            webHookFeedEntry.RequestBody = RequestBody;
            webHookFeedEntry.ResponseHeaders = ResponseHeaders;
            webHookFeedEntry.ResponseBody = ResponseBody;

            return webHookFeedEntry;
        }

        public virtual WebHookFeedEntryEntity FromModel(WebhookFeedEntry webHookFeedEntry, PrimaryKeyResolvingMap pkMap)
        {
            if (webHookFeedEntry == null)
            {
                throw new ArgumentNullException(nameof(webHookFeedEntry));
            }

            Id = webHookFeedEntry.Id;
            CreatedBy = webHookFeedEntry.CreatedBy;
            CreatedDate = webHookFeedEntry.CreatedDate;
            ModifiedBy = webHookFeedEntry.ModifiedBy;
            ModifiedDate = webHookFeedEntry.ModifiedDate;
            WebHookId = webHookFeedEntry.WebHookId;
            EventId = webHookFeedEntry.EventId;
            AttemptCount = webHookFeedEntry.AttemptCount;
            RecordType = webHookFeedEntry.RecordType;
            Status = webHookFeedEntry.Status;
            Error = webHookFeedEntry.Error;
            RequestHeaders = webHookFeedEntry.RequestHeaders;
            RequestBody = webHookFeedEntry.RequestBody;
            ResponseHeaders = webHookFeedEntry.ResponseHeaders;
            ResponseBody = webHookFeedEntry.ResponseBody;

            pkMap.AddPair(webHookFeedEntry, this);

            return this;
        }

        public virtual void Patch(WebHookFeedEntryEntity target)
        {
            target.WebHookId = WebHookId;
            target.EventId = EventId;
            target.AttemptCount = AttemptCount;
            target.Status = Status;
            target.Error = Error;
            target.RequestHeaders = RequestHeaders;
            target.RequestBody = RequestBody;
            target.ResponseHeaders = ResponseHeaders;
            target.ResponseBody = ResponseBody;
            target.RecordType = RecordType;
        }
    }
}
