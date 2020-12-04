using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
    public class WebhookFeedSearchCriteria : SearchCriteriaBase
    {
        public string[] WebHookIds { get; set; }
        public string[] EventIds { get; set; }
        public int[] Statuses { get; set; }
        public int[] RecordTypes { get; set; }
    }
}
