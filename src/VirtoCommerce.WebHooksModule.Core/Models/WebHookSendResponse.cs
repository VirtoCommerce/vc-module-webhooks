using VirtoCommerce.WebhooksModule.Core.Models;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebhookSendResponse
    {
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public WebhookHttpParams ResponseParams { get; set; }
        public bool IsSuccessfull => string.IsNullOrEmpty(Error);
    }
}
