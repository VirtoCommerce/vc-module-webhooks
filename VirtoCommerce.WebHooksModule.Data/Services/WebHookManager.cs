using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;
using VirtoCommerce.WebHooksModule.Data.Utils;

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
        public virtual Task<int> NotifyAsync(string eventId, object eventObject, CancellationToken cancellationToken)
        {
            int skip = 0, take = _webhooksPerButch;
            WebHookSearchResult webHookSearchResult;
            var tasks = new List<Task<WebHookSendResponse>>();

            do
            {
                var criteria = new WebHookSearchCriteria()
                {
                    IsActive = true,
                    EventIds = new[] { eventId },
                    Skip = skip,
                    Take = take,
                };

                webHookSearchResult = _webHookSearchService.Search(criteria);


                // TechDebt: Here we could create a lot of tasks. Need to add throttling. Also need to decrease pool threads usage.
                tasks.AddRange(
                    webHookSearchResult.Results
                        .Select(x => Task.Run(() => NotifyWebHook(eventId, eventObject, x, cancellationToken)))
                        .ToArray()
               );

                skip += take;
            }
            while (webHookSearchResult.TotalCount > skip);

            Task.WaitAll(tasks.ToArray());

            return Task.FromResult(webHookSearchResult.TotalCount);
        }

        /// <inheritdoc />
        public virtual Task<WebHookSendResponse> VerifyWebHookAsync(WebHook webHook)
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

        protected virtual Task HandleEvent(DomainEvent domainEvent, CancellationToken cancellationToken)
        {
            BackgroundJob.Enqueue(() =>
                NotifyAsync(domainEvent.GetType().FullName,
                    domainEvent,
                    cancellationToken));

            return Task.CompletedTask;
        }

        protected virtual async Task<WebHookSendResponse> NotifyWebHook(string eventId, object eventObject, WebHook webHook, CancellationToken cancellationToken)
        {
            WebHookSendResponse response = null;

            var webHookWorkItem = new WebHookWorkItem()
            {
                EventId = eventId,
                WebHook = webHook,
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                webHook.RequestParams = new WebHookHttpParams()
                {
                    Body = eventObject,
                };

                response = await _webHookSender.SendWebHookAsync(webHookWorkItem);

            }
            catch (Exception ex)
            {
                _logger.Log(WebHookFeedUtils.CreateErrorEntry(webHookWorkItem, response, ex.Message));
            }

            return response;
        }
    }
}
