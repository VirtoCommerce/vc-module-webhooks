using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookManager : IWebHookManager
	{
		private readonly IRegisteredEventStore _registeredEventStore;
		private readonly IHandlerRegistrar _eventHandlerRegistrar;
		private readonly IWebHookSearchService _webHookSearchService;

		public WebHookManager(IRegisteredEventStore registeredEventStore,
			IHandlerRegistrar eventHandlerRegistrar,
			IWebHookSearchService webHookSearchService)
		{
			_registeredEventStore = registeredEventStore;
			_eventHandlerRegistrar = eventHandlerRegistrar;
			_webHookSearchService = webHookSearchService;
		}

		/// <inheritdoc />
		public void SubscribeToAllEvents()
		{
			var allRegisteredEvents = _registeredEventStore.GetAllEvents();

			foreach (var registeredEvent in allRegisteredEvents)
			{
				InvokeHandler(registeredEvent.EventType, _eventHandlerRegistrar);
			}
		}

		/// <inheritdoc />
		public Task<int> NotifyAsync(WebHookNotificationRequest notificationRequest)
		{
			// It is just fo the test
			using (EventLog eventLog = new EventLog("Application"))
			{
				eventLog.Source = "Application";
				eventLog.WriteEntry($"WebHook notification sent on event \"{notificationRequest.EventId}\"{Environment.NewLine} Body:{notificationRequest.RequestParams?.Body}", EventLogEntryType.Information, 101, 1);
			}
			return Task.FromResult(1);
		}

		/// <inheritdoc />
		public Task<WebHookResponse> VerifyWebHookAsync(WebHook webHook)
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

		private async Task HandleEvent(DomainEvent domainEvent, CancellationToken cancellationToken)
		{
			// TODO: Need to handle all this work in BackgroundJob. Handler should just schedule the job, all other handling (webhooks search, notification) should be done later in backround.
			var eventId = domainEvent.GetType().FullName;
			var criteria = new WebHookSearchCriteria()
			{
				IsActive = true,
				EventIds = new[] { eventId },
				Skip = 0,
				Take = int.MaxValue,
			};
			// Make batch handling
			var webHookdsSearchResult = _webHookSearchService.Search(criteria);

			foreach (var webHook in webHookdsSearchResult.Results)
			{
				cancellationToken.ThrowIfCancellationRequested();
				await NotifyAsync(new WebHookNotificationRequest()
				{
					EventId = eventId,
					RequestParams = new WebHookHttpParams()
					{
						Body = JsonConvert.SerializeObject(domainEvent),
					}
				});
			}
		}
	}
}
