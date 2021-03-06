using System;
using System.Linq;
using System.Reflection;
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
                .SelectMany(x => GetTypesSafe(x))
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

        private static Type[] GetTypesSafe(Assembly assembly)
        {
            var result = Array.Empty<Type>();

            try
            {
                result = assembly.GetTypes();
            }
            catch (Exception ex) when (ex is ReflectionTypeLoadException || ex is TypeLoadException)
            {
                // No need to trow as we could have exceptions when loading types
            }

            return result;
        }
    }
}
