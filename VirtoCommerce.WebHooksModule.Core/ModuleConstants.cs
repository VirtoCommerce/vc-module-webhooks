namespace VirtoCommerce.WebHooksModule.Core
{
    public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Read = "webhooks:read";
                public const string Update = "webhooks:update";
                public const string Delete = "webhooks:delete";
                public const string ReadFeed = "webhooksFeed:read";
            }
        }

        public static class Log
        {
            public static class Templates
            {
                public const string InvalidHeader = "Could not add header field '{0}' to the WebHook request for WebHook ID '{1}'.";
            }
        }
    }
}
