using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public enum AuthenticationType
    {
        None,
        Basic,
        BearerToken,
        CustomHeader
    }

    public class Webhook : AuditableEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }

        /// <summary>
        /// Authentication type
        /// </summary>
        public AuthenticationType AuthType { get; set; } = AuthenticationType.None;

        /// <summary>
        /// Basic authentication username
        /// </summary>
        public string BasicUsername { get; set; }

        /// <summary>
        /// Basic authentication password
        /// </summary>
        public string BasicPassword { get; set; }

        /// <summary>
        /// Bearer token for BearerToken 
        /// </summary>
        public string BearerToken { get; set; }

        /// <summary>
        /// Name of Custom Http Header.
        /// </summary>
        public string CustomHttpHeaderName { get; set; }

        /// <summary>
        /// Value of Custom Http Header.
        /// </summary>
        public string CustomHttpHeaderValue { get; set; }

        public bool IsActive { get; set; }

        [Obsolete("Use only one event for subscribing. This property would be removed in the future releases.")]
        public bool IsAllEvents { get; set; }

        public long SuccessCount { get; set; }
        public long ErrorCount { get; set; }

        public WebhookEvent[] Events { get; set; }
        public WebhookHttpParams RequestParams { get; set; }
        public WebHookPayload[] Payloads { get; set; }
    }
}
