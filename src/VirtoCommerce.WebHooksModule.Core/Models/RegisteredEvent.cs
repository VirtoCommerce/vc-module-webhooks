using System;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Swagger;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    [SwaggerSchemaId("WebHookRegisteredEvent")]
    public class RegisteredEvent : Entity
    {
        public Type EventType { get; set; }

        public string DisplayName { get; set; }
    }
}
