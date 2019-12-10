using System;
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
        //CodeReview: because of this method will run by Habgfire as background task there are strict  serialization requirements for actual argument
        //it would be better to accept a WebHookRequest here as formal parameter
        public virtual async Task<int> NotifyAsync(string eventId, object eventObject, CancellationToken cancellationToken)
        {
            var result = 0;

            var criteria = new WebHookSearchCriteria()
            {
                IsActive = true,
                EventIds = new[] { eventId },
                Skip = 0,
                Take = int.MaxValue,
            };

            WebHookSendResponse response = null;

            // TechDebt: Make batch handling
            //CodeReview: In addition, you must support throttling for grouped events and batch sending them to a subscribed web hook.
            var webHooksSearchResult = _webHookSearchService.Search(criteria);

            foreach (var webHook in webHooksSearchResult.Results)
            {
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
                        //CodeReview: You must understand well-known event types, such as IChangedEntryEvent, and use it to exclude domain objects and send it as a request payload.
                        Body = eventObject,
                    };

                    response = await _webHookSender.SendWebHookAsync(webHookWorkItem);

                    result++;
                }
                catch (Exception ex)
                {
                    //CodeReview: Log errors in many  places is detected RetriableWebHookSender logs errors as well.
                    _logger.Log(WebHookFeedUtils.CreateErrorEntry(webHookWorkItem, response, ex.Message));
                }
            }

            return result;
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

            //CodeReview: This method will run BackgroundTask for every event in the system it can be resource utilization bottleneck.
            //Better will be checking that given event is has subscriber before schedule background task. (this is required adding caching to the IWebHookSearchService
            BackgroundJob.Enqueue(() =>
                NotifyAsync(domainEvent.GetType().FullName,
                    domainEvent,
                    cancellationToken));

            return Task.CompletedTask;
        }
    }
}
