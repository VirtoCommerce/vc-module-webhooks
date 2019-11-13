using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Data.Services
{
    public class WebhookSearchService : IWebhookSearchService, IWebhookFeedSearchService
    {
        public WebhookSearchResult Search(WebhookSearchCriteria searchCriteria)
        {
            var result = new WebhookSearchResult()
            {
                Results = GetMocks(searchCriteria),
                TotalCount = 100
            };
            return result;
        }

		public WebhookFeedSearchResult Search(WebhookFeedSearchCriteria searchCriteria)
		{
			var result = new WebhookFeedSearchResult()
			{
				Results = GetSampleFeeds(searchCriteria),
				TotalCount = 100
			};
			return result;
		}

		private ICollection<WebhookFeedEntry> GetSampleFeeds(WebhookFeedSearchCriteria searchCriteria)
		{
			var result = new List<WebhookFeedEntry>();

			for (int i = 1; i <= 100; i++)
			{
				result.Add(new WebhookFeedEntry()
				{
					Id = $"id{i}",
					CreatedDate = DateTime.Now.AddMinutes(-i),
				});
			}

			return result.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList();
		}

        private List<Webhook> GetMocks(WebhookSearchCriteria searchCriteria)
        {
            var result = new List<Webhook>();

            for (int i = 1; i <= 100; i++)
            {
                result.Add(new Webhook()
                {
                    Id = $"id{i}",
                    Name = $"Name_{i}",
                    Url = $"https://test-url-{i}",
                    ErrorCount = 3 * i,
                    IsActive = i % 2 == 0,
                    RaisedEventCount = 5 * i
                });
            }

            return result.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList();
        }
    }
}