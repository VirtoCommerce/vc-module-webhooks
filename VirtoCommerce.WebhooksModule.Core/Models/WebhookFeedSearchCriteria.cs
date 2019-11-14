using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebHookFeedSearchCriteria : SearchCriteriaBase
	{
		public string[] WebHookIds { get; set; }
		public string[] EventIds { get; set; }
		public int[] Statuses { get; set; }
	}
}
