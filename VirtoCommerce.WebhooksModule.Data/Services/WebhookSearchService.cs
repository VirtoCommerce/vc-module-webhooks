using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Data.Services
{
    public class WebhookSearchService : IWebhookSearchService
    {
        public GenericSearchResult<WebWebhook> Search(WebhookSearchCriteria searchCriteria)
        {
            var result = new GenericSearchResult<WebWebhook>()
            {
                Results = GetMocks(searchCriteria),
                TotalCount = 100
            };
            return result;
        }

        public GenericSearchResult<WebWebhookFeed> SearchFeed(WebhookSearchCriteria searchCriteria)
        {
            throw new System.NotImplementedException();
        }


        private List<WebWebhook> GetMocks(WebhookSearchCriteria searchCriteria)
        {
            var result = new List<WebWebhook>();

            for (int i = 1; i <= 100; i++)
            {
                result.Add(new WebWebhook()
                {
                    Id = $"id{i}",
                    Name = $"Name_{i}",
                    Url = $"https://test-url-{i}",
                    EventErrorsCount = 3 * i,
                    IsActive = i % 2 == 0,
                    RaisedEventCount = 5 * i
                });
            }

            return result.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList();
        }
    }
}