using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.WebhooksModule.Core.Extensions
{
    public static class DomainEventExtensions
    {
        public static TResult[] GetEntityWithInterface<TResult>(this IEvent obj)
        {
            var result = new List<TResult>();

            var objectType = obj.GetType();
            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var objects = properties.Where(p => (p.Name.Equals(nameof(GenericChangedEntryEvent<TResult>.ChangedEntries)) && p.GetIndexParameters().Length == 0
                                                || p.PropertyType.GetInterfaces().Contains(typeof(TResult))))
                                        .Select(x => x.GetValue(obj, null))
                                        .Where(x => !(x is string));

            foreach (var @object in objects)
            {
                if (@object is IEnumerable enumerable)
                {
                    foreach (var collectionObject in enumerable)
                    {
                        result.AddRange(collectionObject.GetType().GetProperties()
                                                      .Where(x => x.Name.EqualsInvariant(nameof(GenericChangedEntry<TResult>.NewEntry)))
                                                      .Select(p => p.GetValue(collectionObject))
                                                      .OfType<TResult>());
                    }
                }
                else if (@object is TResult concreteObject)
                {
                    result.Add(concreteObject);
                }
            }

            return result.ToArray();
        }

        public static Type GetEntityTypeWithInterface<TResult>(this Type eventType)
        {
            var result = default(Type);
            var properties = eventType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Try to find generic type
            var changedEntryPropertyInfo = properties
                .FirstOrDefault(x =>
                        x.Name.EqualsInvariant(nameof(GenericChangedEntryEvent<TResult>.ChangedEntries)) &&
                        x.GetIndexParameters().Length == 0);

            if (changedEntryPropertyInfo != null)
            {
                // If type is finded, get its type
                result = changedEntryPropertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault()?.GenericTypeArguments?.FirstOrDefault();
            }

            // If previous attempt was failed, try to get from event directly
            if (result is null)
            {
                result = properties.FirstOrDefault(x => x.PropertyType.GetInterfaces().Contains(typeof(TResult)))?.PropertyType;
            }

            return result;
        }
    }
}
