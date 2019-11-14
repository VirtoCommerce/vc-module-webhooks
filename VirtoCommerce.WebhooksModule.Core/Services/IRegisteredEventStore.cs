﻿using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebHooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Services
{
	/// <summary>
	/// Provides an abstraction for getting all registered events.
	/// </summary>
	public interface IRegisteredEventStore
	{
		/// <summary>
		/// Gets all registered <see cref="IEvent"/> instances.
		/// </summary>
		/// <returns>All registered <see cref="WebHook"/> instances for this user.</returns>
		ICollection<RegisteredEvent> GetAllEvents();
	}
}
