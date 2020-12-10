using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Extensions;
using VirtoCommerce.WebhooksModule.Core.Models;
using Xunit;

namespace VirtoCommerce.WebHooksModule.Tests
{
    public class Test
    {
        public Test()
        {
        }

        [Fact]
        public void GetChangedEntriesWithInterface_ReturnEntities()
        {
            //Arrange
            var list = new List<GenericChangedEntry<Webhook>>
            {
                new GenericChangedEntry<Webhook>(new Webhook { Id = Guid.NewGuid().ToString() },
                new Webhook { Id = Guid.NewGuid().ToString() },
                EntryState.Modified)
            };
            var webHookChangedEvent = new WebHookChangedEventFake(list);

            //Act
            var result = webHookChangedEvent.GetChangedEntriesWithInterface<IEntity>();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Length);
        }
    }

    public class WebHookChangedEventFake : GenericChangedEntryEvent<Webhook>
    {
        public WebHookChangedEventFake(IEnumerable<GenericChangedEntry<Webhook>> changedEntries) : base(changedEntries)
        {
        }
    }
}
