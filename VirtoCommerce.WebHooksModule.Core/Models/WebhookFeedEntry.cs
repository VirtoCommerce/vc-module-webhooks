using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebHookFeedEntry : AuditableEntity
    {
        public string WebHookId { get; set; }
        public string EventId { get; set; }
        public int AttemptCount { get; set; }
        public int Status { get; set; }
        public int RecordType { get; set; }
        public string Error { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestBody { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }

        public static WebHookFeedEntry CreateSuccess(string webHookId, string eventId) =>
             new WebHookFeedEntry()
             {
                 RecordType = (int)WebHookFeedEntryType.Success,
                 WebHookId = webHookId,
                 EventId = eventId,
             };

        public static WebHookFeedEntry CreateError(string webHookId,
            string eventId,
            string error,
            int attemptCount = 0,
            int status = 0,
            string requestHeaders = null,
            string requestBody = null,
            string responseHeaders = null,
            string responseBody = null
            ) => new WebHookFeedEntry()
            {
                RecordType = (int)WebHookFeedEntryType.Error,
                WebHookId = webHookId,
                EventId = eventId,
                Error = error,
                AttemptCount = attemptCount,
                Status = status,
                RequestHeaders = requestHeaders,
                RequestBody = requestBody,
                ResponseHeaders = responseHeaders,
                ResponseBody = responseBody,
            };
    }
}
