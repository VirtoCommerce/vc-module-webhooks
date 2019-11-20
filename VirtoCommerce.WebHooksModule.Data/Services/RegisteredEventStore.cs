using System;
using System.Linq;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class RegisteredEventStore : IRegisteredEventStore
    {
        private RegisteredEvent[] _registeredEvents;
        private readonly object _lock = new object();

        public RegisteredEvent[] GetAllEvents()
        {
            if (_registeredEvents == null)
            {
                lock (_lock)
                {
                    if (_registeredEvents == null)
                    {
                        _registeredEvents = DiscoverAllDomainEvents();
                    }
                }

            }
            return _registeredEvents;
        }

        private static RegisteredEvent[] DiscoverAllDomainEvents()
        {
            var eventBaseType = typeof(DomainEvent);

            var result = AppDomain.CurrentDomain.GetAssemblies()
                // Maybe there is a way to find platform- and modules- related assemblies
                .Where(x => !(x.FullName.ToLower().StartsWith("microsoft.") || x.FullName.ToLower().StartsWith("system.")))
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && x.IsSubclassOf(eventBaseType))
                .Select(x => new RegisteredEvent()
                {
                    Id = x.FullName,
                    EventType = x,
                })
                .Distinct()
                .ToArray();
            return result;
        }
    }
}
