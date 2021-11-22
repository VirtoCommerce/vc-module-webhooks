using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
    /// <summary>
    /// Provides an abstraction for getting all registered events.
    /// </summary>
    public interface IRegisteredEventStore
    {
        /// <summary>
        /// Gets all registered <see cref="IEvent"/> instances.
        /// </summary>
        /// <returns>All registered <see cref="RegisteredEvent"/> events.</returns>
        RegisteredEvent[] GetAllEvents();

        /// <summary>
        /// Gets available payload properties for specified eventType
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns>All available properties <see cref="EventObjectProperties"/> .</returns>
        EventObjectProperties GetEventObjectProperties(string eventType);
    }
}
