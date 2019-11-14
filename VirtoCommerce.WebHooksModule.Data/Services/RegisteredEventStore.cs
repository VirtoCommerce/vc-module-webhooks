using System;
using System.Collections.Generic;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class RegisteredEventStore : IRegisteredEventStore
	{
		public ICollection<RegisteredEvent> GetAllEvents()
		{
			return Array.Empty<RegisteredEvent>();
		}
	}
}
