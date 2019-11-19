using System;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class WebHookLogger : IWebHookLogger
    {
        public void Log(WebHookFeedEntry feedEntry)
        {
            System.Diagnostics.Debug.WriteLine($"RecordType: {Enum.GetName(typeof(WebHookFeedEntryType), feedEntry.RecordType)};" +
                $"EventId: {feedEntry.EventId}" +
                $"WebHookId: {feedEntry.WebHookId}" +
                $"RequestBody: {feedEntry.RequestBody}" +
                $"ResponseStatus: {feedEntry.Status}");
        }
    }
}
