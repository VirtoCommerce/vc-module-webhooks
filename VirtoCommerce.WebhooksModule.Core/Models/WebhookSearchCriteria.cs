using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.WebHooksModule.Core.Models
{
	public class WebHookSearchCriteria : SearchCriteriaBase
	{
		public bool IsActive { get; set; }
	}
}