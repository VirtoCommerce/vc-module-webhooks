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
                public const string Execute = "webhooks:execute";
                public const string Update = "webhooks:update";
                public const string Delete = "webhooks:delete";
                public const string ReadFeed = "webhooks:feed:read";
            }
        }
                
        public static class Settings
        {
            public static class General
            {
                public static SettingDescriptor SendRetryCount { get; } = new SettingDescriptor
                {
                    Name = "Webhooks.General.SendRetryCount",
                    ValueType = SettingValueType.Integer,
                    GroupName = "Webhooks|General",
                    IsDictionary = true,
                    DefaultValue = 3
                };

                public static IEnumerable<SettingDescriptor> AllSettings => new List<SettingDescriptor> { SendRetryCount };
            }
        }
    }
}
