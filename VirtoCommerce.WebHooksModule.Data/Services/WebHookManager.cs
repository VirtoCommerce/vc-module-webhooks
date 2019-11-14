using System;
using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookManager : IWebHookManager
	{
		public Task<int> NotifyAsync(WebHookNotificationRequest notificationRequest)
		{
			throw new NotImplementedException();
		}

		public Task SubscribeToAll()
		{
			throw new NotImplementedException();
		}

		public Task<WebHookResponse> VerifyWebHookAsync(WebHook webHook)
		{
			throw new NotImplementedException();
		}
	}
}
