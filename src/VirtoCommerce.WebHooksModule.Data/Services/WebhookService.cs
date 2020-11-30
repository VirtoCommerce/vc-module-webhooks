using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
	public class WebHookService : IWebHookSearchService, IWebHookService
    {
        private readonly Func<IWebHookRepository> _webHookRepositoryFactory;
        private readonly IWebHookFeedReader _feedReader;

        public WebHookService(Func<IWebHookRepository> webHookRepositoryFactory, IWebHookFeedReader feedReader)
        {
            _webHookRepositoryFactory = webHookRepositoryFactory;
            _feedReader = feedReader;
        }

        public async Task DeleteByIdsAsync(string[] ids)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                await repository.DeleteWebHooksByIdsAsync(ids);
                await repository.UnitOfWork.CommitAsync();

                ResetCache();
            }
        }

        public async Task<WebHook[]> GetByIdsAsync(string[] ids)
        {
            var result = new List<WebHook>();

            if (!ids.IsNullOrEmpty())
            {
                using (var repository = _webHookRepositoryFactory())
                {
                    var entities = await repository.GetWebHooksByIdsAsync(ids);

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

        public async Task SaveChangesAsync(WebHook[] webHooks)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<WebHook>>();

            using (var repository = _webHookRepositoryFactory())
            {
                var existingIds = webHooks.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var originalEntities = await repository.GetWebHooksByIdsAsync(existingIds);

                foreach (var webHook in webHooks)
                {
                    var originalEntity = originalEntities.FirstOrDefault(x => x.Id == webHook.Id);
                    var modifiedEntity = AbstractTypeFactory<WebHookEntity>.TryCreateInstance().FromModel(webHook, pkMap);

                    if (originalEntity != null)
                    {
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

                await repository.UnitOfWork.CommitAsync();
                ResetCache();

                pkMap.ResolvePrimaryKeys();
            }
        }

        public async Task<WebHookSearchResult> SearchAsync(WebHookSearchCriteria searchCriteria)
        {
            //return _cacheManager.Get($"{nameof(ModuleConstants.WebhooksSearchCacheRegion)}-{searchCriteria.GetCacheKey()}", ModuleConstants.WebhooksSearchCacheRegion, () =>
            {
                var result = new WebHookSearchResult();

                using (var repository = _webHookRepositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var query = BuildQuery(searchCriteria, repository);

                    result.TotalCount = await query.CountAsync();

                    if (searchCriteria.Take > 0 && result.TotalCount > 0)
                    {
                        var sortInfos = searchCriteria.SortInfos;

                        if (sortInfos.IsNullOrEmpty())
                        {
                            sortInfos = new[] { new SortInfo { SortColumn = ReflectionUtility.GetPropertyName<WebHookEntity>(x => x.Name), SortDirection = SortDirection.Descending } };
                        }

                        query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);

                        var webHookIds = query.Select(x => x.Id).Skip(searchCriteria.Skip).Take(searchCriteria.Take).ToArray();

                        result.Results = (await GetByIdsAsync(webHookIds)).OrderBy(x => Array.IndexOf(webHookIds, x.Id)).ToArray();
                    }
                }

                return result;
            }
        }

        protected virtual IQueryable<WebHookEntity> BuildQuery(WebHookSearchCriteria searchCriteria, IWebHookRepository repository)
        {
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

            return query;
        }
        protected virtual void ResetCache()
        {
            //TODO
            //_cacheManager.ClearRegion(ModuleConstants.WebhooksSearchCacheRegion);
        }
    }
}
