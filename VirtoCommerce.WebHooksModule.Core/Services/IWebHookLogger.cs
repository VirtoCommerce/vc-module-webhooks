using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	public interface IWebHookLogger
	{
		void Log(WebHookFeedEntry feedEntry);
    }
}
