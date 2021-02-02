using System;
using System.Collections;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Extensions;
using Xunit;

namespace VirtoCommerce.WebHooksModule.Tests
{
    public class WebHookEventTests
    {
        public WebHookEventTests()
        {
        }

        [Theory]
        [ClassData(typeof(WebHookTestData))]
        public void GetChangedEntriesWithInterface_ReturnEntities(DomainEvent domainEvent)
        {
            //Arrange
            
            //Act
            var result = domainEvent.GetEntityWithInterface<IEntity>();

            //Assert
            Assert.NotNull(result);
            Assert.All(result, item => Assert.IsAssignableFrom<IEntity>(item));
        }

        [Theory]
        [ClassData(typeof(WebHookTestValueObjects))]
        public void GetChangedEntriesWithInterface_ReturnValueObjects(DomainEvent domainEvent)
        {
            //Arrange

            //Act
            var result = domainEvent.GetEntityWithInterface<IValueObject>();

            //Assert
            Assert.NotNull(result);
            Assert.All(result, item => Assert.IsAssignableFrom<IValueObject>(item));
        }
    }

    class WebHookTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new WebHookChangedEventFake(new List<GenericChangedEntry<FakeEntity>>
                    {
                        new GenericChangedEntry<FakeEntity>(new FakeEntity { Id = Guid.NewGuid().ToString() },
                        new FakeEntity { Id = Guid.NewGuid().ToString() },
                        EntryState.Modified),
                        new GenericChangedEntry<FakeEntity>(new FakeEntity { Id = Guid.NewGuid().ToString() },
                        EntryState.Added),

                    })
                };
            yield return new object[] { new WebHookObjectEventFake(new FakeEntity { Id = Guid.NewGuid().ToString() }) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class WebHookTestValueObjects : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {
                new WebHookChangedEventFakeValueObject(new List<GenericChangedEntry<FakeValueObject>>
                    {
                        new GenericChangedEntry<FakeValueObject>(new FakeValueObject { Name = "Name", Status = "State" },
                        new FakeValueObject { Name = "OldName", Status = "OldState" },
                        EntryState.Modified),
                        new GenericChangedEntry<FakeValueObject>(new FakeValueObject(), EntryState.Added),

                    })
                };
            yield return new object[] { new WebHookObjectEventFakeValueObject(new FakeValueObject { Name = "Name", Status = "State" }) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class WebHookChangedEventFake : GenericChangedEntryEvent<FakeEntity>
    {
        public WebHookChangedEventFake(IEnumerable<GenericChangedEntry<FakeEntity>> changedEntries) : base(changedEntries)
        {
        }
    }

    public class WebHookChangedEventFakeValueObject : GenericChangedEntryEvent<FakeValueObject>
    {
        public WebHookChangedEventFakeValueObject(IEnumerable<GenericChangedEntry<FakeValueObject>> changedEntries) : base(changedEntries)
        {
        }
    }

    public class WebHookObjectEventFake : DomainEvent
    {
        public WebHookObjectEventFake(FakeEntity value)
        {
            Value = value;
        }

        public FakeEntity Value { get; set; }
    }

    public class WebHookObjectEventFakeValueObject : DomainEvent
    {
        public WebHookObjectEventFakeValueObject(FakeValueObject value)
        {
            Value = value;
        }

        public FakeValueObject Value { get; set; }
    }



    public class FakeEntity : IEntity
    {
        public string Id { get; set; }
    }

    public class FakeValueObject : ValueObject
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
