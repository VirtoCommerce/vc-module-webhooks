using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Moq;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Bus;
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
            var mockedRegisteredEventStore = GetMockedRegisteredEventStore();
            var mockedHandlerRegistrar = GetMockedHandlerRegistrar();
            var mockedWebHookSearchService = GetMockedWebHookSearchService();
            var mockedBackgroundJobClient = GetMockedBackgroundJobClient();

            var requestBody = string.Empty;
            var mockedWebHookSender = GetMockedWebHookSender();
            mockedWebHookSender.Setup(x => x.SendWebHookAsync(It.IsAny<WebhookWorkItem>())).Callback<WebhookWorkItem>(
                x =>
                {
                    requestBody = x.WebHook.RequestParams.Body;
                });

            var webhookManager = new WebHookManager(mockedRegisteredEventStore, mockedHandlerRegistrar,
                mockedWebHookSearchService, mockedWebHookSender.Object, mockedBackgroundJobClient);

            var webhookRequest = new WebhookRequest
            {
                WebHooks = new List<Webhook>
                {
                    new Webhook
                    {
                        Payloads = new [] { new WebHookPayload
                        {
                            EventPropertyName = "Number",
                        }},
                    }
                },
                DomainEventObject = new FakeEvent(new[]{ new GenericChangedEntry<FakeEntity>(new FakeEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = 15M
                }, EntryState.Added)}),
            };

            // Act
            await webhookManager.NotifyAsync(webhookRequest, CancellationToken.None);
            var result = JArray.Parse(requestBody).First;


            // Assert
            Assert.Equal(typeof(FakeEntity).FullName, result["ObjectType"].ToString());
            Assert.Equal("15", result["Number"].ToString());
        }

        private IRegisteredEventStore GetMockedRegisteredEventStore()
        {
            var result = new Mock<IRegisteredEventStore>();

            return result.Object;
        }

        private IHandlerRegistrar GetMockedHandlerRegistrar()
        {
            var result = new Mock<IHandlerRegistrar>();

            return result.Object;
        }

        private IWebHookSearchService GetMockedWebHookSearchService()
        {
            var result = new Mock<IWebHookSearchService>();

            return result.Object;
        }

        private Mock<IWebHookSender> GetMockedWebHookSender()
        {
            var result = new Mock<IWebHookSender>();

            return result;
        }

        private IBackgroundJobClient GetMockedBackgroundJobClient()
        {
            var result = new Mock<IBackgroundJobClient>();

            return result.Object;
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
