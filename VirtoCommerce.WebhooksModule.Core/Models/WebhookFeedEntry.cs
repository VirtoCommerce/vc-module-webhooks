using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebHookFeedEntry : AuditableEntity
	{
		public string WebHookId { get; set; }
		public string EventId { get; set; }
		public int AttemptCount { get; set; }
		public int Status { get; set; }
		public string Error { get; set; }
		public string RequestHeader { get; set; }
		public string RequestBody { get; set; }
		public string ResponseHeader { get; set; }
		public string ResponseBody { get; set; }
	}
}