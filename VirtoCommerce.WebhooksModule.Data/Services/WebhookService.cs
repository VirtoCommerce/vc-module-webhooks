using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Data.Services
{
	public class WebhookService : IWebhookSearchService, IWebhookService
	{
		public void DeleteByIds(string[] ids)
		{
			throw new NotImplementedException();
		}

		public Webhook[] GetByIds(string[] ids)
		{
			return new[] { new Webhook()
			{
				Id = "test",
				IsActive = false,
				Name = "test",
				Url = "https://myLAUrl",
				RaisedEventCount = 100500,
				ErrorCount = 500
			}};
		}

		public void SaveChanges(Webhook[] webhooks)
		{
			throw new NotImplementedException();
		}

		public WebhookSearchResult Search(WebhookSearchCriteria searchCriteria)
		{
			var result = new WebhookSearchResult()
			{
				Results = GetMocks(searchCriteria),
				TotalCount = 100
			};
			return result;
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
					RaisedEventCount = 5 * i,
					IsAllEvents = i % 2 == 1,
					Events = i % 2 == 0
						? new[] { new WebhookEvent() {
							Id = $"event_1",
							WebhookId = $"id{i}",
							EventId = "OrderChangedEvent",
						}}
						: null,
				});
			}

			return result.Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToList();
		}
	}
}