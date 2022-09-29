using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;
using VirtoCommerce.WebHooksModule.Data.Services;
using Xunit;

namespace VirtoCommerce.WebhooksModule.Tests
{
    public class WebhookEventPayloadTests
    {
        [Fact]
        public async Task PayloadExtracting_SuccessfulExtractedPayload()
        {
            // Arrange
            var mockedRegisteredEventStore = new Mock<IRegisteredEventStore>();

            mockedRegisteredEventStore.Setup(x => x.GetAllEvents()).Returns(new[] { new RegisteredEvent { EventType = typeof(FakeEvent), Id = new Guid().ToString() } });

            var fakeHandlerRegistrar = new FakeHandlerRegistrar();

            var mockedWebHookSearchService = new Mock<IWebHookSearchService>();

            mockedWebHookSearchService.Setup(x => x.SearchAsync(It.IsAny<WebhookSearchCriteria>())).ReturnsAsync(
                new WebhookSearchResult
                {
                    Results = new List<Webhook>
                    {
                        new Webhook
                        {
                            Id = "webhook1",
                            Payloads = new[] { new WebHookPayload { EventPropertyName = "Number", } },
                        }
                    },
                    TotalCount = 1,
                });

            var mockedBackgroundJobClient = new Mock<IBackgroundJobClient>();

            var webHookRequest = default(WebhookRequest);

            mockedBackgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((x, s) =>
                {
                    webHookRequest = (WebhookRequest)x.Args.First();
                });

            var requestBody = string.Empty;
            var mockedWebHookSender = new Mock<IWebHookSender>();

            var webhookManager = new WebHookManager(mockedRegisteredEventStore.Object, fakeHandlerRegistrar,
                mockedWebHookSearchService.Object, mockedWebHookSender.Object, mockedBackgroundJobClient.Object);

            // Act
            webhookManager.SubscribeToAllEvents();

            var eventHandler = fakeHandlerRegistrar.Handlers.First();

            await ((Func<FakeEvent, CancellationToken, Task>)eventHandler).Invoke(new FakeEvent(new[]{ new GenericChangedEntry<FakeEntity>(new FakeEntity
            {
                Id = Guid.NewGuid().ToString(),
                Number = 15M,
                Values = new []{ "Value1", "Value2" },
            }, EntryState.Added)}), CancellationToken.None);

            var result = JArray.Parse(webHookRequest.WebhooksPayload.First().Value).First;

            // Assert
            Assert.Equal(typeof(FakeEntity).FullName, result["ObjectType"].ToString());
            Assert.Equal("15", result[nameof(FakeEntity.Number)].ToString());
            Assert.NotNull(result[nameof(FakeEntity.Id)]);
            Assert.Null(result[nameof(FakeEntity.Values)]);
        }

        [Fact]
        public async Task PayloadExtracting_OldEntry_SuccessfulExtractedPayload()
        {
            // Arrange
            var mockedRegisteredEventStore = new Mock<IRegisteredEventStore>();

            mockedRegisteredEventStore.Setup(x => x.GetAllEvents()).Returns(new[] { new RegisteredEvent { EventType = typeof(FakeEvent), Id = new Guid().ToString() } });

            var fakeHandlerRegistrar = new FakeHandlerRegistrar();

            var mockedWebHookSearchService = new Mock<IWebHookSearchService>();

            mockedWebHookSearchService.Setup(x => x.SearchAsync(It.IsAny<WebhookSearchCriteria>())).ReturnsAsync(
                new WebhookSearchResult
                {
                    Results = new List<Webhook>
                    {
                        new Webhook
                        {
                            Id = "webhook1",
                            Payloads = new[] { new WebHookPayload { EventPropertyName = "Number", } },
                        }
                    },
                    TotalCount = 1,
                });

            var mockedBackgroundJobClient = new Mock<IBackgroundJobClient>();

            var webHookRequest = default(WebhookRequest);

            mockedBackgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback<Job, IState>((x, s) =>
                {
                    webHookRequest = (WebhookRequest)x.Args.First();
                });

            var requestBody = string.Empty;
            var mockedWebHookSender = new Mock<IWebHookSender>();

            var webhookManager = new WebHookManager(mockedRegisteredEventStore.Object, fakeHandlerRegistrar,
                mockedWebHookSearchService.Object, mockedWebHookSender.Object, mockedBackgroundJobClient.Object);

            // Act
            webhookManager.SubscribeToAllEvents();

            var eventHandler = fakeHandlerRegistrar.Handlers.First();

            var entityId = Guid.NewGuid().ToString();

            await ((Func<FakeEvent, CancellationToken, Task>)eventHandler).Invoke(new FakeEvent(new[]{ new GenericChangedEntry<FakeEntity>(new FakeEntity
            {
                Id = entityId,
                Number = 15M,
                Values = new []{ "Value1", "Value2" },
            },
            new FakeEntity
            {
                Id = Guid.NewGuid().ToString(),
                Number = 14M,
                Values = new []{ "Value1", "Value2" },
            }
            , EntryState.Modified)}), CancellationToken.None);

            var result = JArray.Parse(webHookRequest.WebhooksPayload.First().Value).First;

            // Assert
            Assert.Equal(typeof(FakeEntity).FullName, result["ObjectType"].ToString());
            Assert.Equal("15", result[nameof(FakeEntity.Number)].ToString());
            Assert.Equal("14", result.SelectToken("__Previous.Number").ToString());
            Assert.NotNull(result[nameof(FakeEntity.Id)]);
            Assert.Null(result[nameof(FakeEntity.Values)]);
        }

        public class FakeEntity : IEntity
        {
            public string Id { get; set; }
            public decimal Number { get; set; }
            public string[] Values { get; set; }
        }

        public class FakeEvent : GenericChangedEntryEvent<FakeEntity>
        {
            public FakeEvent(IEnumerable<GenericChangedEntry<FakeEntity>> changedEntries)
            : base(changedEntries)
            {

            }
        }
    }
}
