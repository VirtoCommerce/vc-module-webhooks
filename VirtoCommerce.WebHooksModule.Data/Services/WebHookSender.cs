using System;
using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookSender : IWebHookSender
	{
		public Task<WebHookResponse> SendWebHookAsync(WebHook webHook)
		{
			throw new NotImplementedException();
		}
	}
}
