namespace VirtoCommerce.WebhooksModule.Core.Models
{
    public enum WebhookResponseGroup
    {
        Info,
        WithFeed,
        Full = Info | WithFeed
    }
}
