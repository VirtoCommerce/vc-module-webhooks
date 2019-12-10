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
    //CodeReview: Need to add caching for all Get/Search methods with cache eviction on changes.
    public class WebHookService : ServiceBase, IWebHookSearchService, IWebHookService
    {
        private readonly Func<IWebHookRepository> _webHookRepositoryFactory;
        private readonly IWebHookFeedReader _feedReader;


        public WebHookService(Func<IWebHookRepository> webHookRepositoryFactory, IWebHookFeedReader feedReader)
        {
            _webHookRepositoryFactory = webHookRepositoryFactory;
            _feedReader = feedReader;
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

                foreach (var webHook in result)
                {
                    webHook.SuccessCount = _feedReader.GetSuccessCount(webHook.Id);
                    webHook.ErrorCount = _feedReader.GetErrorCount(webHook.Id);
                }
            }

            return result.ToArray();
        }

        public void SaveChanges(WebHook[] webHooks)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<WebHook>>();

            using (var repository = _webHookRepositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var existingIds = webHooks.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var originalEntities = repository.GetWebHooksByIds(existingIds).ToList();

                foreach (var webHook in webHooks)
                {
                    var originalEntity = originalEntities.FirstOrDefault(x => x.Id == webHook.Id);
                    var modifiedEntity = AbstractTypeFactory<WebHookEntity>.TryCreateInstance().FromModel(webHook, pkMap);

                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        changedEntries.Add(new GenericChangedEntry<WebHook>(webHook, originalEntity.ToModel(new WebHook()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);

                        //Force set ModifiedDate property to mark a webHook changed. Special for update when only event list has changes and webHook table hasn't any changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<WebHook>(webHook, EntryState.Added));
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
                    //CodeReview: Name.ToLower() is redundant because SQL all strings compare as case insensitive 
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
                    //CodeReview: You can use nameof instead of ReflectionUtility.GetPropertyName
                    sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<WebHookEntity>(x => x.Name), SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

                var webHookIds = query.Select(x => x.Id).Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToArray();
                result.Results = GetByIds(webHookIds).OrderBy(x => Array.IndexOf(webHookIds, x.Id)).ToArray();
            }

            return result;
        }
    }
}
