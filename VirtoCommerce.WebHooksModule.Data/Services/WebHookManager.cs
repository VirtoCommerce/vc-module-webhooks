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

            WebHookResponse response = null;

            // TechDebt: Make batch handling
            var webHookdsSearchResult = _webHookSearchService.Search(criteria);

            foreach (var webHook in webHookdsSearchResult.Results)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    webHook.RequestParams = new WebHookHttpParams()
                    {
                        Body = eventObject,
                    };

                    response = await _webHookSender.SendWebHookAsync(new WebHookWorkItem()
                    {
                        EventId = eventId,
                        WebHook = webHook,
                        Offset = 0,
                    });

                    if (!response.IsSuccessfull)
                    {
                        _logger.Log(WebHookFeedUtils.CreateErrorEntry(response.Error, eventId, response, webHook));
                    }
                    else
                    {
                        _logger.Log(WebHookFeedUtils.CreateSuccessEntry(eventId, response, webHook));
                    }

                    result++;
                }
                catch (Exception ex)
                {
                    _logger.Log(WebHookFeedUtils.CreateErrorEntry(ex.Message, eventId, response, webHook));
                }
            }

            return result;
        }

        /// <inheritdoc />
        public virtual Task<WebHookResponse> VerifyWebHookAsync(WebHook webHook)
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
    }
}
