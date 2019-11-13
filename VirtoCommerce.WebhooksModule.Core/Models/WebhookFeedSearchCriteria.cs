using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebhookFeedSearchCriteria : SearchCriteriaBase
	{
		public string[] WebhookIds { get; set; }
		public string[] EventIds { get; set; }
		public int[] Statuses { get; set; }
	}
}
