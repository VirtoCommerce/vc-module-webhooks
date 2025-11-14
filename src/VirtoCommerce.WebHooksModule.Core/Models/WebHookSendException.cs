using System;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    [Serializable]
    public class WebHookSendException : Exception
    {
        public WebHookSendException(string message, string webHookId, string eventId) : base(message)
        {
            WebHookId = webHookId;
            EventId = eventId;
        }

        public string WebHookId { get; set; }
        public string EventId { get; set; }
    }
}
