using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class WebhookSearchCriteria : SearchCriteriaBase
	{
		public bool? IsActive { get; set; }
		public string[] EventIds { get; set; }
        //TODO need to remove after refactoring working with IBackgroundJob(VP-6287)
        public bool ForceCacheReset { get; set; }    
	}
}
