using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private readonly IRegisteredEventStore _registeredEventStore;
        private readonly IHandlerRegistrar _eventHandlerRegistrar;
        private readonly IWebHookSearchService _webHookSearchService;
        private readonly IWebHookSender _webHookSender;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WebHookManager(IRegisteredEventStore registeredEventStore,
            IHandlerRegistrar eventHandlerRegistrar,
            IWebHookSearchService webHookSearchService,
            IWebHookSender webHookSender,
            IBackgroundJobClient backgroundJobClient)
        {
            _registeredEventStore = registeredEventStore;
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _webHookSearchService = webHookSearchService;
            _webHookSender = webHookSender;
            _backgroundJobClient = backgroundJobClient;
        }

        /// <inheritdoc />
        public virtual void SubscribeToAllEvents()
        {
            var allRegisteredEvents = _registeredEventStore.GetAllEvents();

            foreach (var registeredEvent in allRegisteredEvents)
            {
                InvokeHandler(registeredEvent.EventType, _eventHandlerRegistrar);
            }
        }

        /// <inheritdoc />
        [DisableConcurrentExecution(2)]
        public virtual async Task<int> NotifyAsync(WebhookRequest request, CancellationToken cancellationToken)
        {
            var webhooksCount = request.WebHooks.Count;
            var tasks = new List<Task<WebhookSendResponse>>();

            for (var i = 0; i < webhooksCount; i += _webhooksPerButch)
            {
                // TechDebt: Here we could create a lot of tasks. Need to add throttling. Also need to decrease pool threads usage.
                tasks.AddRange(request.WebHooks.Skip(i).Take(_webhooksPerButch)
                        .Select(x => Task.Run(() => NotifyWebHook(request.EventId, request.DomainEventObject, x, cancellationToken)))
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


        private void InvokeHandler(Type eventType, IHandlerRegistrar registrar)
        {
            var registerExecutorMethod = registrar
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.Name == nameof(IHandlerRegistrar.RegisterHandler))
                .Where(mi => mi.IsGenericMethod)
                .Where(mi => mi.GetGenericArguments().Length == 1)
                .Single(mi => mi.GetParameters().Length == 1)
                .MakeGenericMethod(eventType);

            Func<DomainEvent, CancellationToken, Task> del = (x, token) =>
            {
                return HandleEvent(x, token);
            };

            registerExecutorMethod.Invoke(registrar, new object[] { del });
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
                var request = new WebhookRequest
                {
                    EventId = eventId,
                    DomainEventObject = domainEvent,
                    WebHooks = webHookSearchResult.Results
                };

                _backgroundJobClient.Schedule(() => NotifyAsync(request, cancellationToken), TimeSpan.FromMinutes(1));
            }
        }

        // Introduced breaking change
        protected virtual async Task<WebhookSendResponse> NotifyWebHook(string eventId, DomainEvent domainEvent, Webhook webHook, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            WebhookSendResponse response = null;

            var webHookWorkItem = new WebhookWorkItem()
            {
                EventId = eventId,
                WebHook = webHook,
            };

            var webhookPayload = new List<Dictionary<string, JToken>>();

            var entities = domainEvent.GetEntityWithInterface<IEntity>();

            foreach (var entity in entities)
            {
                var jObject = JObject.FromObject(entity);
                var currentResult = new Dictionary<string, JToken>();

                currentResult.Add("ObjectType", JToken.FromObject(entity.GetType().FullName));

                foreach (var webHookEventPayloadProperty in webHook.EventPayloadProperties.Union(new[] { "Id" }))
                {
                    currentResult.Add(webHookEventPayloadProperty, jObject.SelectToken($"$.{webHookEventPayloadProperty}"));
                }

                webhookPayload.Add(currentResult);
            }

            webHook.RequestParams = new WebhookHttpParams()
            {
                Body = JsonConvert.SerializeObject(webhookPayload),
            };

            response = await _webHookSender.SendWebHookAsync(webHookWorkItem);

            return response;
        }
    }
}
