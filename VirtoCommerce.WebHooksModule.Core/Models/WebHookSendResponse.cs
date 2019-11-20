namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebHookSendResponse
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public WebHookHttpParams ResponseParams { get; set; }
        public bool IsSuccessfull => string.IsNullOrEmpty(Error);
    }
}
