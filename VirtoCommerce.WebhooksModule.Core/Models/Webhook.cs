﻿using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebHook : AuditableEntity
	{
		public string Name { get; set; }
		public string Url { get; set; }
		public string ContentType { get; set; }
		public bool IsActive { get; set; }
		public bool IsAllEvents { get; set; }
		public long RaisedEventCount { get; set; }
		public long ErrorCount { get; set; }
		public WebHookEvent[] Events { get; set; }
		public WebHookHttpParams RequestParams { get; set; }
	}
}