using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.WebHooksModule.Core
{
	public static class ModuleConstants
    {
        public static class Security
        {
            public static class Permissions
            {
                public const string Access = "webhooks:access";
                public const string Read = "webhooks:read";
                public const string Update = "webhooks:update";
                public const string Delete = "webhooks:delete";
                public const string ReadFeed = "webhooks:feed:read";

                public static string[] AllPermissions { get; } = { Access, Read, Update, Delete, ReadFeed };
            }
        }
                
        public static class Settings
        {
            public static class General
            {
                private const int DefaultSendRetryCount = 3;
                private const int DefaultLatestErrorCount = 5;

                public static SettingDescriptor SendRetryCount { get; } = new SettingDescriptor
                {
                    Name = "Webhooks.General.SendRetryCount",
                    ValueType = SettingValueType.Integer,
                    GroupName = "Webhooks|General",
                    DefaultValue = DefaultSendRetryCount
                };

                public static SettingDescriptor LatestErrorCount { get; } = new SettingDescriptor
                {
                    Name = "Webhooks.General.LatestErrorCount",
                    ValueType = SettingValueType.Integer,
                    GroupName = "Webhooks|General",
                    DefaultValue = DefaultLatestErrorCount
                };

                public static IEnumerable<SettingDescriptor> AllSettings => new List<SettingDescriptor> { SendRetryCount, LatestErrorCount };
            }
        }
    }
}
