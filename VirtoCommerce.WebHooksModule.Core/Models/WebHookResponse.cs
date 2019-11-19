namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebHookResponse
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public WebHookHttpParams ResponseParams { get; set; }
        public WebHookFeedEntry FeedEntry { get; set; }
        public bool IsSuccessfull => string.IsNullOrEmpty(Error);
    }
}
