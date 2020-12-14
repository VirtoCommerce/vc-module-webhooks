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
        public static TResult[] GetChangedEntriesWithInterface<TResult>(this IEvent obj, string kindEntry)
        {
            var result = new List<TResult>();

            var objectType = obj.GetType();
            var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //Handle collection and arrays
            var collections = properties.Where(p => p.Name.Equals(nameof(GenericChangedEntryEvent<TResult>.ChangedEntries)) && p.GetIndexParameters().Length == 0)
                                        .Select(x => x.GetValue(obj, null))
                                        .Where(x => x is IEnumerable && !(x is string))
                                        .Cast<IEnumerable>();

            foreach (var collection in collections)
            {
                foreach (var collectionObject in collection)
                {
                    foreach (var pi in collectionObject.GetType().GetProperties().Where(x => x.Name.EqualsInvariant(kindEntry)))
                    {
                        if (pi.PropertyType.GetInterfaces().Contains(typeof(TResult)))
                        {
                            result.Add((TResult)pi.GetValue(collectionObject));
                        }
                    }

                }
            }

            return result.ToArray();
        }
    }    
}
