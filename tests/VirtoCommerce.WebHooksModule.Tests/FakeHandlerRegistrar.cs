using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.WebhooksModule.Tests
{
    public sealed class FakeHandlerRegistrar : IEventHandlerRegistrar
    {
        public List<object> Handlers { get; internal set; } = [];

        public void RegisterEventHandler<T>(Func<T, Task> handler) where T : IEvent
        {
            Handlers.Add(handler);
        }

        public void RegisterEventHandler<T>(Func<T, CancellationToken, Task> handler) where T : IEvent
        {
            Handlers.Add(handler);
        }

        public void RegisterEventHandler<T>(IEventHandler<T> handler) where T : IEvent
        {
            Handlers.Add(handler);
        }

        public void RegisterEventHandler<T>(ICancellableEventHandler<T> handler) where T : IEvent
        {
            Handlers.Add(handler);
        }

        public void UnregisterEventHandler<T>(Type handlerType = null) where T : IEvent
        {
            throw new NotImplementedException();
        }

        public void UnregisterAllEventHandlers()
        {
            throw new NotImplementedException();
        }
    }
}
