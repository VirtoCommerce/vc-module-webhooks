using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookService : IWebHookSearchService, IWebHookService
	{
		public void DeleteByIds(string[] ids)
		{
			throw new NotImplementedException();
		}

		public WebHook[] GetByIds(string[] ids)
		{
			return new[] { new WebHook()
			{
				Id = "test",
				IsActive = false,
				Name = "test",
				Url = "https://myLAUrl",
				RaisedEventCount = 100500,
				ErrorCount = 500
			}};
		}

		public void SaveChanges(WebHook[] webhooks)
		{
			throw new NotImplementedException();
		}

		public WebHookSearchResult Search(WebHookSearchCriteria searchCriteria)
		{
			var result = new WebHookSearchResult()
			{
				Results = GetMocks(searchCriteria),
				TotalCount = 100
			};
			return result;
		}

		private List<WebHook> GetMocks(WebHookSearchCriteria searchCriteria)
		{
			var result = new List<WebHook>();

			for (int i = 1; i <= 100; i++)
			{
				result.Add(new WebHook()
				{
					Id = $"id{i}",
					Name = $"Name_{i}",
					Url = $"https://test-url-{i}",
					ErrorCount = 3 * i,
					IsActive = i % 2 == 0,
					RaisedEventCount = 5 * i,
					IsAllEvents = i % 2 == 1,
					Events = i % 2 == 0
						? new[] { new WebHookEvent() {
							Id = $"event_1",
							WebHookId = $"id{i}",
							EventId = "OrderChangedEvent",
						}}
						: null,
				});
			}

			return result.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList();
		}
	}
}