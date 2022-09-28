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
            Assert.All(result, item => Assert.IsAssignableFrom<IEntity>(item.NewEntry));
        }

        [Fact]
        public void GetVoidWithObjectTypePassedIntoTheConstructor_ReturnEmptyCollection()
        {
            // Arrange
            var eventWithObjectInjectedIntoConstructor = new WebHookCtorObjectEventFake(new FakeEntity());

            // Act
            var result = eventWithObjectInjectedIntoConstructor.GetEntityWithInterface<IEntity>();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetEntityTypeViaEventType_ReturnEntityType()
        {
            // Arrange

            // Act
            var entityTypeFromGenericEvent = typeof(WebHookChangedEventFake).GetEntityTypeWithInterface<IEntity>();
            var entityTypeFromEventInheritedViaDomainEvent = typeof(WebHookObjectEventFake).GetEntityTypeWithInterface<IEntity>();
            var wrongType = typeof(WebHookObjectEventFake).GetEntityTypeWithInterface<IWrong>();
            var correctTypeEvenWithIncorrectInterface = typeof(WebHookChangedEventFake).GetEntityTypeWithInterface<IWrong>();

            // Assert
            Assert.Equal(nameof(FakeEntity), entityTypeFromEventInheritedViaDomainEvent.Name);
            Assert.Equal(nameof(FakeEntity), entityTypeFromGenericEvent.Name);
            Assert.Null(wrongType);
            Assert.Equal(nameof(FakeEntity), correctTypeEvenWithIncorrectInterface.Name);
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

    public class WebHookChangedEventFake : GenericChangedEntryEvent<FakeEntity>
    {
        public WebHookChangedEventFake(IEnumerable<GenericChangedEntry<FakeEntity>> changedEntries) : base(changedEntries)
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

    public class WebHookCtorObjectEventFake : DomainEvent
    {
        public WebHookCtorObjectEventFake(FakeEntity value)
        {

        }
    }

    public class FakeEntity : IEntity
    {
        public string Id { get; set; }
    }

    public interface IWrong
    {

    }
}
