using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookService : ServiceBase, IWebHookSearchService, IWebHookService
	{
		private readonly Func<IWebHookRepository> _webHookRepositoryFactory;


		public WebHookService(Func<IWebHookRepository> webHookRepositoryFactory)
		{
			_webHookRepositoryFactory = webHookRepositoryFactory;
		}

		public void DeleteByIds(string[] ids)
		{
			using (var repository = _webHookRepositoryFactory())
			{
				repository.DeleteWebHooksByIds(ids);
				repository.UnitOfWork.Commit();
			}
		}

		public WebHook[] GetByIds(string[] ids)
		{
			var result = new List<WebHook>();

			if (!ids.IsNullOrEmpty())
			{
				using (var repository = _webHookRepositoryFactory())
				{
					var entities = repository.GetWebHooksByIds(ids);

					if (!entities.IsNullOrEmpty())
					{
						result.AddRange(entities.Select(x => x.ToModel(AbstractTypeFactory<WebHook>.TryCreateInstance())));
					}
				}
			}

			return result.ToArray();
		}

		public void SaveChanges(WebHook[] webhooks)
		{
			var pkMap = new PrimaryKeyResolvingMap();
			var changedEntries = new List<GenericChangedEntry<WebHook>>();

			using (var repository = _webHookRepositoryFactory())
			using (var changeTracker = GetChangeTracker(repository))
			{
				var existingIds = webhooks.Where(x => !x.IsTransient()).Select(x => x.Id);
				var existingEntities = repository.WebHooks.Where(x => existingIds.Contains(x.Id));

				foreach (var webhook in webhooks)
				{
					var changedEntity = new WebHookEntity();
					changedEntity.FromModel(webhook, pkMap);

					var existingEntity = existingEntities.FirstOrDefault(x => x.Id == webhook.Id);

					if (existingEntity != null)
					{
						changeTracker.Attach(existingEntity);
						changedEntries.Add(new GenericChangedEntry<WebHook>(webhook, existingEntity.ToModel(new WebHook()), EntryState.Modified));
						changedEntity.Patch(existingEntity);

					}
					else
					{
						repository.Add(changedEntity);
						changedEntries.Add(new GenericChangedEntry<WebHook>(webhook, EntryState.Added));

					}
				}
				CommitChanges(repository);
				pkMap.ResolvePrimaryKeys();
			}
		}

		public WebHookSearchResult Search(WebHookSearchCriteria searchCriteria)
		{
			var result = new WebHookSearchResult();

			using (var repository = _webHookRepositoryFactory())
			{
				repository.DisableChangesTracking();

				var query = repository.WebHooks;

				if (searchCriteria.IsActive.HasValue)
				{
					query = repository.WebHooks.Where(x => x.IsActive == searchCriteria.IsActive);
				}

				if (!string.IsNullOrWhiteSpace(searchCriteria.SearchPhrase))
				{
					query = query.Where(x => x.Name.ToLower().Contains(searchCriteria.SearchPhrase.ToLower()));
				}

				if (!searchCriteria.EventIds.IsNullOrEmpty())
				{
					query = query.Where(x => x.IsAllEvents || x.Events.Any(y => searchCriteria.EventIds.Contains(y.EventId)));
				}

				result.TotalCount = query.Count();

				var sortInfos = searchCriteria.SortInfos;
				if (sortInfos.IsNullOrEmpty())
				{
					sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<WebHookEntity>(x => x.Name), SortDirection = SortDirection.Descending } };
				}
				query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

				var webHookIds = query.Select(x => x.Id).Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToArray();
				result.Results = GetByIds(webHookIds);
			}

			return result;
		}

	}
}