namespace VirtoCommerce.WebHooksModule.Core.Services
{
    public interface IWebHookFeedReader
    {
        int GetSuccessCount(string webHookId);
        int GetErrorCount(string webHookId);
    }
}
