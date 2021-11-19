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

        // CAS security attribute is obsolete and removed accordingly to https://docs.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0003#workarounds
        protected WebHookSendException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            WebHookId = info.GetString("WebHookSendException.WebHookId");
            EventId = info.GetString("WebHookSendException.ValidationErrors");
        }

        // CAS security attribute is obsolete and removed accordingly to https://docs.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0003#workarounds
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("WebHookSendException.WebHookId", this.WebHookId);
            info.AddValue("WebHookSendException.EventId", this.EventId);
        }
    }
}
