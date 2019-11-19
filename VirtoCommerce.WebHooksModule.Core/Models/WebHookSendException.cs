using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

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

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        protected WebHookSendException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            WebHookId = info.GetString("WebHookSendException.WebHookId");
            EventId = info.GetString("WebHookSendException.ValidationErrors");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("WebHookSendException.WebHookId", this.WebHookId);
            info.AddValue("WebHookSendException.EventId", this.EventId);
        }
    }
}
