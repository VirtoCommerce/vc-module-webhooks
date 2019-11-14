using System.Collections.Generic;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class WebHookHttpParams
	{
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
		public string Body { get; set; }
	}
}
