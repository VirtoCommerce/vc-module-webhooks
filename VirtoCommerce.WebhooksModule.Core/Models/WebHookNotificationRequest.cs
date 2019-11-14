namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class WebHookNotificationRequest
	{
		public string EventId { get; set; }
		public WebHookHttpParams RequestParams { get; set; }
	}
}
