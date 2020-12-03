using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Events;
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
        private readonly IWebHookLogger _logger;

        public WebHookManager(IRegisteredEventStore registeredEventStore,
            IHandlerRegistrar eventHandlerRegistrar,
            IWebHookSearchService webHookSearchService,
            IWebHookSender webHookSender,
            IWebHookLogger logger)
        {
            _registeredEventStore = registeredEventStore;
            _eventHandlerRegistrar = eventHandlerRegistrar;
            _webHookSearchService = webHookSearchService;
            _webHookSender = webHookSender;
            _logger = logger;
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
        public virtual Task<int> NotifyAsync(WebhookRequest request, CancellationToken cancellationToken)
        {
            var webhooksCount = request.WebHooks.Count;
            var tasks = new List<Task<WebhookSendResponse>>();

            for (var i = 0; i < webhooksCount; i += _webhooksPerButch)
            {
                // TechDebt: Here we could create a lot of tasks. Need to add throttling. Also need to decrease pool threads usage.
                tasks.AddRange(request.WebHooks.Skip(i).Take(_webhooksPerButch)
                        .Select(x => Task.Run(() => NotifyWebHook(request.EventId, request.EventObject, x, cancellationToken)))
                        .ToArray());
            }

            Task.WaitAll(tasks.ToArray());

            return Task.FromResult(request.WebHooks.Count);
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
                .Where(mi => mi.Name == "RegisterHandler")
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
                var request = new WebhookRequest { EventId = eventId, EventObject = domainEvent, WebHooks = webHookSearchResult.Results };
                BackgroundJob.Enqueue(() => NotifyAsync(request, cancellationToken));
            }
        }

        protected virtual async Task<WebhookSendResponse> NotifyWebHook(string eventId, object eventObject, Webhook webHook, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            WebhookSendResponse response = null;

            var webHookWorkItem = new WebhookWorkItem()
            {
                EventId = eventId,
                WebHook = webHook,
            };

            webHook.RequestParams = new WebhookHttpParams()
            {
                Body = eventObject,
            };

            response = await _webHookSender.SendWebHookAsync(webHookWorkItem);

            return response;
        }
    }
}
