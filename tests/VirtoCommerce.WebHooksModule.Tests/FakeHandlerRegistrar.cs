using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Messages;

namespace VirtoCommerce.WebhooksModule.Tests
{
    public class FakeHandlerRegistrar : IHandlerRegistrar
    {
        public ICollection<object> Handlers { get; set; } = new List<object>();

        public void RegisterHandler<T>(Func<T, CancellationToken, Task> handler) where T : class, IMessage
        {
            Handlers.Add(handler);
        }
    }
}
