using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public class WebhookHttpParams
    {
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public object Body { get; set; }
    }
}
