using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class EventObjectProperties
    {
        public bool Discovered { get; set; }
        public List<string> Properties { get; set; }
    }
}
