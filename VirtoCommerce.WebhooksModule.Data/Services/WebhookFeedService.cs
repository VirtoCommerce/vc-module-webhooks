using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Data.Services
{
	public class WebhookFeedService : IWebhookFeedService, IWebhookFeedSearchService
	{
		public void DeleteByIds(string[] ids)
		{
			throw new System.NotImplementedException();
		}

		public WebhookFeedEntry[] GetByIds(string[] ids)
		{
			throw new System.NotImplementedException();
		}

		public void SaveChanges(WebhookFeedEntry[] webhookLogEntries)
		{
			throw new System.NotImplementedException();
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

	}
}
