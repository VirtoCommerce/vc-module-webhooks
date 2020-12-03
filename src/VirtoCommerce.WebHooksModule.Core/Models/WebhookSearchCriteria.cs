using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class WebhookSearchCriteria : SearchCriteriaBase
	{
		public bool? IsActive { get; set; }
		public string[] EventIds { get; set; }
	}
}
