using System.Threading.Tasks;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
    /// <summary>
    /// Provides an abstraction for sending out WebHooks.
    /// </summary>
    public interface IWebHookSender
    {
        /// <summary>
        /// Sends out the webhook.
        /// </summary>
        /// <param name="webHook">WebHook with filled <see cref="WebHook.RequestParams"/> to send.</param>
        Task<WebHookSendResponse> SendWebHookAsync(WebHookWorkItem webHookWorkItem);
    }
}
