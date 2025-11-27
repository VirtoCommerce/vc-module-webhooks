using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Extensions;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class RegisteredEventStore : IRegisteredEventStore
    {
        private static readonly string[] _ignoredProperties = new[] { "Id" };

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

        public EventObjectProperties GetEventObjectProperties(string eventType)
        {
            var domainEventType = GetAllEvents().FirstOrDefault(x => x.Id.EqualsIgnoreCase(eventType))?.EventType ?? throw new InvalidOperationException($"The domain event \"{eventType}\" is not registered");

            var eventObjectType = domainEventType.GetEntityTypeWithInterface<IEntity>();

            // For abstract event type entity only common properties would be able
            if (eventObjectType != null && !eventObjectType.IsAbstract)
            {
                var actualType = typeof(AbstractTypeFactory<>).MakeGenericType(eventObjectType).GetMethod("FindTypeInfoByName")?.Invoke(null, new[] { eventObjectType.Name }) as TypeInfo<IEntity>;

                if (actualType != null)
                {
                    eventObjectType = actualType.Type;
                }
            }

            var result = eventObjectType?.GetProperties()
                .Where(x => !_ignoredProperties.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                .Select(x => x.Name).OrderBy(x => x)
                ?.ToList() ?? new List<string>();

            // If result is empty, it means that for some reason the entity type doesn't contains any properties which could be used as webhook payload
            return new EventObjectProperties { Discovered = result.Count != 0, Properties = result };
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
                    DisplayName = DomainEventExtensions.ResolveDisplayName(x.FullName)
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
