using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.WebhooksModule.Core.Models
{
	public class WebHookSearchCriteria : SearchCriteriaBase
	{
		public bool IsActive { get; set; }
	}
}