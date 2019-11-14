using System.Collections.Generic;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebHookHttpParams
	{
		public Dictionary<string, string> Headers { get; set; }
		public string Body { get; set; }
	}
}
