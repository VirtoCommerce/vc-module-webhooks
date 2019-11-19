using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookFeedService : IWebHookFeedService, IWebHookFeedSearchService
	{
		public void DeleteByIds(string[] ids)
		{
			throw new System.NotImplementedException();
		}

		public WebHookFeedEntry[] GetByIds(string[] ids)
		{
			throw new System.NotImplementedException();
		}

		public void SaveChanges(WebHookFeedEntry[] webhookLogEntries)
		{
			throw new System.NotImplementedException();
		}

		public WebHookFeedSearchResult Search(WebHookFeedSearchCriteria searchCriteria)
		{
			var result = new WebHookFeedSearchResult()
			{
				Results = GetSampleFeeds(searchCriteria),
				TotalCount = 100
			};
			return result;
		}

		private ICollection<WebHookFeedEntry> GetSampleFeeds(WebHookFeedSearchCriteria searchCriteria)
		{
			var result = new List<WebHookFeedEntry>();

			for (int i = 1; i <= 100; i++)
			{
				result.Add(new WebHookFeedEntry()
				{
					Id = $"id{i}",
					CreatedDate = DateTime.Now.AddMinutes(-i),
				});
			}

			return result.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList();
		}

	}
}
