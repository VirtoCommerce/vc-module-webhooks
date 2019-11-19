using Newtonsoft.Json.Linq;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Data.Utils
{
    public static class WebHookFeedUtils
    {
        public static WebHookFeedEntry CreateErrorEntry(string message, string eventId, WebHookResponse response, WebHook webHook)
        {
            return CreateFeedEntry(WebHookFeedEntryType.Error, eventId, response, webHook, message);
        }

        public static WebHookFeedEntry CreateSuccessEntry(string eventId, WebHookResponse response, WebHook webHook)
        {
            return CreateFeedEntry(WebHookFeedEntryType.Success, eventId, response, webHook);
        }

        public static WebHookFeedEntry CreateFeedEntry(WebHookFeedEntryType entryType, string eventId, WebHookResponse response, WebHook webHook, string error = null)
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
