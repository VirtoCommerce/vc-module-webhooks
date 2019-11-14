using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookManager : IWebHookManager
	{
		private readonly IRegisteredEventStore _registeredEventStore;
		private readonly IHandlerRegistrar _eventHandlerRegistrar;

		public WebHookManager(IRegisteredEventStore registeredEventStore, IHandlerRegistrar eventHandlerRegistrar)
		{
			_registeredEventStore = registeredEventStore;
			_eventHandlerRegistrar = eventHandlerRegistrar;
		}

		public void SubscribeToAllEvents()
		{
			var allRegisteredEvents = _registeredEventStore.GetAllEvents();

			//foreach (var registeredEvent in allRegisteredEvents)
			//{
			//	var eventType = registeredEvent.EventType;
			//	var genericRegisterHandlerMethod = _eventHandlerRegistrar.GetType()
			//		.GetMethods()
			//		.FirstOrDefault(x => x.Name.EqualsInvariant("RegisterHandler") && x.GetParameters().Length == 1)
			//		.MakeGenericMethod(eventType);

			//	var getHandlderFuncGenericMethod = GetType()
			//		.GetMethods()
			//		.FirstOrDefault(x => x.Name.EqualsInvariant("GetHandlderFunc") && x.GetParameters().Length == 1)
			//		.MakeGenericMethod(eventType);

			//	var handlerFuncInstance = getHandlderFuncGenericMethod.Invoke(null, null);

			//	genericRegisterHandlerMethod.Invoke(_eventHandlerRegistrar, new object[] { handlerFuncInstance });
			//}
		}

		public Task<int> NotifyAsync(WebHookNotificationRequest notificationRequest)
		{
			throw new NotImplementedException();
		}

		public Task<WebHookResponse> VerifyWebHookAsync(WebHook webHook)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Constructs generic function of type <see cref="Func&lt;T, CancellationToken, Task&gt;>"/> from the generic event type.
		/// Used in <see cref="SubscribeToAllEvents"/> through Reflection.
		/// </summary>
		/// <typeparam name="T">Generic event type.</typeparam>
		/// <returns>Handler function.</returns>
		private Func<T, CancellationToken, Task> GetHandlderFunc<T>()
		{
			var eventType = typeof(T);
			var eventId = eventType.Name;

			return (Func<T, CancellationToken, Task>)Delegate.CreateDelegate(typeof(T), new Func<T, CancellationToken, Task>(async (message, token) =>
			{
				await NotifyAsync(new WebHookNotificationRequest()
				{
					EventId = eventId,
					RequestParams = new WebHookHttpParams()
					{
						Body = JsonConvert.SerializeObject(message),
					}
				});
			}).Method);
		}
	}
}
