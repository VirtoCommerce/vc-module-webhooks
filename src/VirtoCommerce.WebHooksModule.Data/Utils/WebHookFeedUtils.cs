using System.Linq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Data.Utils
{
    public static class WebHookFeedUtils
    {
        public static WebhookFeedEntry CreateErrorEntry(WebhookWorkItem webHookWorkItem, WebhookSendResponse response)
        {
            return CreateFeedEntry(WebhookFeedEntryType.Error, webHookWorkItem.EventId, response, webHookWorkItem.WebHook);
        }

        public static WebhookFeedEntry CreateSuccessEntry(WebhookWorkItem webHookWorkItem, WebhookSendResponse response)
        {
            return CreateFeedEntry(WebhookFeedEntryType.Success, webHookWorkItem.EventId, response, webHookWorkItem.WebHook);
        }

        public static WebhookFeedEntry CreateFeedEntry(WebhookFeedEntryType entryType, string eventId, WebhookSendResponse response, Webhook webHook)
        {
            var result = new WebhookFeedEntry()
            {
                RecordType = (int)entryType,
                WebHookId = webHook.Id,
                EventId = eventId,
                AttemptCount = 0,
                Error = new string(response.Error.Take(1024).ToArray()),
                Status = response?.StatusCode ?? 0,
                RequestHeaders = GetJsonString(webHook.RequestParams.Headers),
                RequestBody = webHook.RequestParams.Body,
                ResponseHeaders = GetJsonString(response?.ResponseParams?.Headers),
                ResponseBody = response?.ResponseParams?.Body,
            };

            return result;
        }

        static string GetJsonString(object obj)
        {
            return obj != null ? JObject.FromObject(obj).ToString() : null;
        }
    }
}
