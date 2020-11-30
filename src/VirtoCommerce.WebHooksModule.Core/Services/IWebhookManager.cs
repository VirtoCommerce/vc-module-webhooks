using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	/// <summary>
	/// Provides an abstraction for launching Webhooks based on events happening in the system. When 
	/// the <see cref="NotifyAsync(DomainEvent domainEvent, CancellationToken cancellationToken)"/> method is called, 
	/// all registered WebHooks with matching eventId will launch indicating to the recipient of the WebHook that an event happened.
	/// </summary>
	public interface IWebHookManager
    {
        /// <summary>
        /// Subscribes to all events that could occur in the system with the handler that will call webhook notification.
        /// </summary>
        /// <returns></returns>
        void SubscribeToAllEvents();

        /// <summary>
        /// Verifies that the URI of the given <paramref name="webhook"/> is reachable and responds with the expected
        /// data in response to an echo request. If a correct response can not be obtained then an <see cref="System.Exception"/>
        /// is thrown with a detailed description of the problem.
        /// </summary>
        /// <param name="webHook">The <see cref="WebHook"/> with filled <see cref="WebHookHttpParams"/> to verify</param>
        Task<WebHookSendResponse> VerifyWebHookAsync(WebHook webHook);

        /// <summary>
        /// Submits a notification to the given WebHooks with eventId like <paramref name="eventId"/>. 
        /// For active WebHooks, an HTTP request will be sent to the designated WebHook URI with information about the action.
        /// </summary>
        /// <param name="eventId">Event id which needs to be checked for existing active WebHooks.</param>
        /// <param name="eventObject">Event data to send.</param>
        /// <param name="webhooks">Webhooks to notify.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of <see cref="WebHook"/> instances that were selected and subsequently notified about the actions.</returns>
        Task<int> NotifyAsync(string eventId, object eventObject, ICollection<WebHook> webhooks, CancellationToken cancellationToken);
    }
}
