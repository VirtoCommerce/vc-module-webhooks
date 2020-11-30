using System;
using System.Collections.Generic;
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
    public class WebHookFeedService : ServiceBase, IWebHookFeedService, IWebHookFeedSearchService, IWebHookFeedReader
    {
        private readonly Func<IWebHookRepository> _webHookRepositoryFactory;

        public WebHookFeedService(Func<IWebHookRepository> webHookRepositoryFactory)
        {
            _webHookRepositoryFactory = webHookRepositoryFactory;
        }

        public void DeleteByIds(string[] ids)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                repository.DeleteWebHookFeedEntriesByIds(ids);
                repository.UnitOfWork.Commit();
            }
        }

        public WebHookFeedEntry[] GetByIds(string[] ids)
        {
            var result = new List<WebHookFeedEntry>();

            if (!ids.IsNullOrEmpty())
            {
                using (var repository = _webHookRepositoryFactory())
                {
                    var entities = repository.GetWebHookFeedEntriesByIds(ids);

                    if (!entities.IsNullOrEmpty())
                    {
                        result.AddRange(entities.Select(x => x.ToModel(AbstractTypeFactory<WebHookFeedEntry>.TryCreateInstance())));
                    }
                }
            }

            return result.ToArray();
        }

        public void SaveChanges(WebHookFeedEntry[] webhookLogEntries)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<WebHookFeedEntry>>();

            using (var repository = _webHookRepositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existingIds = webhookLogEntries.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var originalEntities = repository.GetWebHookFeedEntriesByIds(existingIds).ToList();

                foreach (var webHookFeedEntry in webhookLogEntries)
                {
                    var originalEntity = originalEntities.FirstOrDefault(x => x.Id == webHookFeedEntry.Id);
                    var modifiedEntity = AbstractTypeFactory<WebHookFeedEntryEntity>.TryCreateInstance().FromModel(webHookFeedEntry, pkMap);

                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        changedEntries.Add(new GenericChangedEntry<WebHookFeedEntry>(webHookFeedEntry, originalEntity.ToModel(AbstractTypeFactory<WebHookFeedEntry>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<WebHookFeedEntry>(webHookFeedEntry, EntryState.Added));
                    }
                }
                CommitChanges(repository);
                pkMap.ResolvePrimaryKeys();
            }
        }

        public WebHookFeedSearchResult Search(WebHookFeedSearchCriteria searchCriteria)
        {
            var result = new WebHookFeedSearchResult();

            using (var repository = _webHookRepositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.WebHookFeedEntries;

                if (!string.IsNullOrWhiteSpace(searchCriteria.SearchPhrase))
                {
                    query = query.Where(x => x.EventId.ToLower().Contains(searchCriteria.SearchPhrase.ToLower()));
                }

                if (!searchCriteria.WebHookIds.IsNullOrEmpty())
                {
                    query = query.Where(x => searchCriteria.WebHookIds.Contains(x.WebHookId));
                }

                if (!searchCriteria.EventIds.IsNullOrEmpty())
                {
                    query = query.Where(x => searchCriteria.EventIds.Contains(x.EventId));
                }

                if (!searchCriteria.RecordTypes.IsNullOrEmpty())
                {
                    query = query.Where(x => searchCriteria.RecordTypes.Contains(x.RecordType));
                }

                if (!searchCriteria.Statuses.IsNullOrEmpty())
                {
                    query = query.Where(x => searchCriteria.Statuses.Contains(x.Status));
                }

                result.TotalCount = query.Count();

                var sortInfos = searchCriteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<WebHookFeedEntryEntity>(x => x.CreatedDate), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

                var webHookFeedIds = query.Select(x => x.Id).Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToArray();
                result.Results = GetByIds(webHookFeedIds);
            }
            return result;
        }

        #region IWebHookFeedReader

        public int GetSuccessCount(string webHookId)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                return repository.WebHookFeedEntries.FirstOrDefault(x => x.WebHookId == webHookId && x.RecordType == (int)WebHookFeedEntryType.Success)?.AttemptCount ?? 0;
            }
        }

        public int GetErrorCount(string webHookId)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                return repository.WebHookFeedEntries.Count(x => x.WebHookId == webHookId && x.RecordType == (int)WebHookFeedEntryType.Error);
            }
        }

        #endregion
    }
}
