using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class RegisteredEvent : Entity
	{
		public Type EventType { get; set; }
	}
}
