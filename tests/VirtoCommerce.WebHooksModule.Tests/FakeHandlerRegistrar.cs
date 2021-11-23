using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Messages;

namespace VirtoCommerce.WebhooksModule.Tests
{
    public sealed class FakeHandlerRegistrar : IHandlerRegistrar
    {
        public List<object> Handlers { get; internal set; } = new List<object>();

        public void RegisterHandler<T>(Func<T, CancellationToken, Task> handler) where T : class, IMessage
        {
            Handlers.Add(handler);
        }
    }
}
