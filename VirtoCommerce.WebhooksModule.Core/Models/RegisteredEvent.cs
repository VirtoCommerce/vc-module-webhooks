using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class RegisteredEvent : IEntity
	{
		public string Id { get; set; }
		public Type EventType { get; set; }
	}
}
