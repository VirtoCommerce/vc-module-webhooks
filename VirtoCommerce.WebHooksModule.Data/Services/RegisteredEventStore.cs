using System;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class RegisteredEventStore : IRegisteredEventStore
	{
		public RegisteredEvent[] GetAllEvents()
		{
			return Array.Empty<RegisteredEvent>();
		}
	}
}
