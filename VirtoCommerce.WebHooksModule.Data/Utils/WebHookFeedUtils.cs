using Newtonsoft.Json.Linq;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Data.Utils
{
    public static class WebHookFeedUtils
    {
        public static WebHookFeedEntry CreateErrorEntry(WebHookWorkItem webHookWorkItem, WebHookSendResponse response, string errorMessage)
        {
            return CreateFeedEntry(WebHookFeedEntryType.Error, webHookWorkItem.EventId, response, webHookWorkItem.WebHook, errorMessage);
        }

        public static WebHookFeedEntry CreateSuccessEntry(WebHookWorkItem webHookWorkItem, WebHookSendResponse response)
        {
            return CreateFeedEntry(WebHookFeedEntryType.Success, webHookWorkItem.EventId, response, webHookWorkItem.WebHook);
        }

        public static WebHookFeedEntry CreateFeedEntry(WebHookFeedEntryType entryType, string eventId, WebHookSendResponse response, WebHook webHook, string error = null)
        {
            var result = new WebHookFeedEntry()
            {
                RecordType = (int)entryType,
                WebHookId = webHook.Id,
                EventId = eventId,
                AttemptCount = 0,
                Error = error,
                Status = response?.StatusCode ?? 0,
                RequestHeaders = GetJsonString(webHook.RequestParams.Headers),
                RequestBody = GetJsonString(webHook.RequestParams.Body),
                ResponseHeaders = GetJsonString(response?.ResponseParams?.Headers),
                ResponseBody = GetJsonString(response?.ResponseParams?.Body),
            };

            return result;
        }

        static string GetJsonString(object obj)
        {
            return obj != null ? JObject.FromObject(obj).ToString() : null;
        }
    }
}
