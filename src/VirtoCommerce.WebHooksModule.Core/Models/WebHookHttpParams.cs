using System.Collections.Generic;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebHookHttpParams
    {
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public object Body { get; set; }
    }
}
