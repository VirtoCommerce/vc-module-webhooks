using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Extensions;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class WebHookManager : IWebHookManager
    {
        private const int _webhooksPerButch = 20;

        private readonly IHandlerRegistrar _eventHandlerRegistrar;
        private readonly IWebHookSearchService _webHookSearchService;
        private readonly IWebHookSender _webHookSender;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WebHookManager(
            IHandlerRegistrar eventHandlerRegistrar,
            IWebHookSearchService webHookSearchService,
            IWebHookSender webHookSender,
            IBackgroundJobClient backgroundJobClient)
        {
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _webHookSearchService = webHookSearchService;
            _webHookSender = webHookSender;
            _backgroundJobClient = backgroundJobClient;
        }

        /// <inheritdoc />
        public virtual void SubscribeToAllEvents()
        {
            _eventHandlerRegistrar.RegisterHandler<DomainEvent>(HandleEvent);
        }

        public virtual async Task<int> NotifyAsync(WebhookRequest request, CancellationToken cancellationToken)
        {
            var webhooksCount = request.WebHooks.Count;
            var tasks = new List<Task<WebhookSendResponse>>();

            for (var i = 0; i < webhooksCount; i += _webhooksPerButch)
            {
                // TechDebt: Here we could create a lot of tasks. Need to add throttling. Also need to decrease pool threads usage.
                tasks.AddRange(request.WebHooks.Skip(i).Take(_webhooksPerButch)
                        .Select(x => Task.Run(() => NotifyWebHook(request.EventId, request.WebhooksPayload?.FirstOrDefault(r => r.Key.EqualsInvariant(x.Id)).Value, x, cancellationToken)))
                        .ToArray());
            }

            await Task.WhenAll(tasks);

            return request.WebHooks.Count;
        }

        /// <inheritdoc />
        public virtual Task<WebhookSendResponse> VerifyWebHookAsync(Webhook webHook)
        {
            throw new NotImplementedException();
        }


        protected virtual async Task HandleEvent(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var eventId = domainEvent.GetType().FullName;
            var criteria = new WebhookSearchCriteria()
            {
                IsActive = true,
                EventIds = new[] { eventId },
                Skip = 0,
                Take = int.MaxValue,
                ResponseGroup = WebhookResponseGroup.Info.ToString()
            };

            var webHookSearchResult = await _webHookSearchService.SearchAsync(criteria);

            if (webHookSearchResult.TotalCount > 0)
            {
                var webhooksPayload = new Dictionary<string, string>();
                var entities = domainEvent.GetEntityWithInterface<IEntity>();

                foreach (var webHook in webHookSearchResult.Results)
                {
                    var entityPayload = ResolveEventPropertiesFromEventEntities(webHook, entities);
                    webhooksPayload.Add(webHook.Id, JsonConvert.SerializeObject(entityPayload));
                }

                var request = new WebhookRequest
                {
                    EventId = eventId,
                    WebhooksPayload = webhooksPayload,
                    WebHooks = webHookSearchResult.Results
                };

                _backgroundJobClient.Schedule(() => NotifyAsync(request, cancellationToken), TimeSpan.FromSeconds(5));
            }
        }

        private static List<Dictionary<string, JToken>> ResolveEventPropertiesFromEventEntities(Webhook webHook, DomainEventObject<IEntity>[] entities)
        {
            var entityPayload = new List<Dictionary<string, JToken>>();

            foreach (var entity in entities)
            {
                var currentResult = ResolveEventPropertiesFromEventEntity(webHook, entity);
                entityPayload.Add(currentResult);
            }

            return entityPayload;
        }

        private static Dictionary<string, JToken> ResolveEventPropertiesFromEventEntity(Webhook webHook, DomainEventObject<IEntity> entity)
        {
            var jObject = JObject.FromObject(entity.NewEntry);
            var currentResult = new Dictionary<string, JToken>
            {
                // Add Generic Properties ObjectType and Id
                { "ObjectType", JToken.FromObject(entity.NewEntry.GetType().FullName) },
                { "Id", JToken.FromObject(jObject.SelectToken("$.Id")) }
            };

            // Add Rroperties  properties from new entity
            foreach (var webHookEventPayloadProperty in webHook.Payloads.Select(x => x.EventPropertyName))
            {
                currentResult.Add(webHookEventPayloadProperty, jObject.SelectToken($"$.{webHookEventPayloadProperty}"));

            }

            // Add Rroperties from new old entity
            if (entity.OldEntry != null)
            {
                var oldEntryObject = new JObject();
                var jOldObject = JObject.FromObject(entity.OldEntry);
                foreach (var webHookEventPayloadProperty in webHook.Payloads.Select(x => x.EventPropertyName))
                {
                    oldEntryObject[webHookEventPayloadProperty] = jOldObject.SelectToken($"$.{webHookEventPayloadProperty}");
                }
                currentResult.Add("__Previous", oldEntryObject);
            }

            return currentResult;
        }

        protected virtual async Task<WebhookSendResponse> NotifyWebHook(string eventId, string webHookPayload, Webhook webHook, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var webHookWorkItem = new WebhookWorkItem()
            {
                EventId = eventId,
                WebHook = webHook,
            };

            webHook.RequestParams = new WebhookHttpParams
            {
                Body = webHookPayload,
            };

            var response = await _webHookSender.SendWebHookAsync(webHookWorkItem);

            return response;
        }
    }
}
