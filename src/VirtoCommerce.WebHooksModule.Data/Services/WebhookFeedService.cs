using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class WebHookFeedService : IWebHookFeedService, IWebHookFeedSearchService, IWebHookFeedReader
    {
        private readonly Func<IWebHookRepository> _webHookRepositoryFactory;

        public WebHookFeedService(Func<IWebHookRepository> webHookRepositoryFactory)
        {
            _webHookRepositoryFactory = webHookRepositoryFactory;
        }

        public async Task DeleteByIdsAsync(string[] ids)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                await repository.DeleteWebHookFeedEntriesByIdsAsync(ids);
                repository.UnitOfWork.Commit();
            }
        }

        public async Task<WebhookFeedEntry[]> GetByIdsAsync(string[] ids)
        {
            var result = new List<WebhookFeedEntry>();

            if (!ids.IsNullOrEmpty())
            {
                using (var repository = _webHookRepositoryFactory())
                {
                    var entities = await repository.GetWebHookFeedEntriesByIdsAsync(ids);

                    if (!entities.IsNullOrEmpty())
                    {
                        result.AddRange(entities.Select(x => x.ToModel(AbstractTypeFactory<WebhookFeedEntry>.TryCreateInstance())));
                    }
                }
            }

            return result.ToArray();
        }

        public async Task SaveChangesAsync(WebhookFeedEntry[] webhookLogEntries)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<WebhookFeedEntry>>();

            using (var repository = _webHookRepositoryFactory())
            {
                var existingIds = webhookLogEntries.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray();
                var originalEntities = await repository.GetWebHookFeedEntriesByIdsAsync(existingIds);

                foreach (var webHookFeedEntry in webhookLogEntries)
                {
                    var originalEntity = originalEntities.FirstOrDefault(x => x.Id == webHookFeedEntry.Id);
                    var modifiedEntity = AbstractTypeFactory<WebHookFeedEntryEntity>.TryCreateInstance().FromModel(webHookFeedEntry, pkMap);

                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<WebhookFeedEntry>(webHookFeedEntry, originalEntity.ToModel(AbstractTypeFactory<WebhookFeedEntry>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<WebhookFeedEntry>(webHookFeedEntry, EntryState.Added));
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
            }
        }

        public async Task<WebHookFeedSearchResult> SearchAsync(WebhookFeedSearchCriteria searchCriteria)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                repository.DisableChangesTracking();

                var result = AbstractTypeFactory<WebHookFeedSearchResult>.TryCreateInstance();

                var sortInfos = BuildSortExpression(searchCriteria);
                var query = BuildQuery(repository, searchCriteria);

                result.TotalCount = await query.CountAsync();


                if (searchCriteria.Take > 0)
                {
                    var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(searchCriteria.Skip).Take(searchCriteria.Take)
                                        .ToArrayAsync();

                    result.Results = (await GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                }

                return result;
            }
        }

        #region IWebHookFeedReader

        public async Task<IDictionary<string, int>> GetSuccessCountsAsync(string[] webHookIds)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                var feedEntiries = await repository.WebHookFeedEntries
                    .Where(x => webHookIds.Contains(x.WebHookId)
                        && x.RecordType == (int)WebhookFeedEntryType.Success
                        && x.AttemptCount > 0)
                    .GroupBy(p => p.WebHookId)
                    .Select(g => new
                    {
                        WebHookId = g.Key,
                        Count = g.Sum(x => x.AttemptCount)
                    })
                    .ToDictionaryAsync(k => k.WebHookId, v => v.Count);
                return feedEntiries;
            }
        }

        public async Task<IDictionary<string, int>> GetErrorCountsAsync(string[] webHookIds)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                var result = await repository.WebHookFeedEntries
                    .Where(x => webHookIds.Contains(x.WebHookId) && x.RecordType == (int)WebhookFeedEntryType.Error)
                    .GroupBy(p => p.WebHookId)
                    .Select(g => new
                    {
                        WebHookId = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(k => k.WebHookId, v => v.Count);
                return result;
            }
        }

        public async Task UpdateCountAttemps(WebhookFeedEntry[] webhookLogEntries)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _webHookRepositoryFactory())
            {
                await repository.UpdateAttemptCountsAsync(
                    webhookLogEntries.Select(x => AbstractTypeFactory<WebHookFeedEntryEntity>.TryCreateInstance().FromModel(x, pkMap))
                                     .ToArray());
                repository.UnitOfWork.Commit();
            }
        }

        public async Task<string[]> GetAllErrorEntriesExceptLatestAsync(string[] webHookIds, int exceptLatestCount)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                var result = (await repository.WebHookFeedEntries
                                            .Where(x => webHookIds.Contains(x.WebHookId) && x.RecordType == (int)WebhookFeedEntryType.Error)
                                            .Select(x => new { x.Id, x.WebHookId, x.CreatedDate })
                                            .ToListAsync())
                                 .GroupBy(x => x.WebHookId)
                                 .Where(x => x.Count() > exceptLatestCount)
                                 .SelectMany(x => x.OrderByDescending(d => d.CreatedDate).Skip(exceptLatestCount).Select(y => y.Id))
                                 .ToArray();

                return result;
            }
        }

        #endregion


        protected virtual IQueryable<WebHookFeedEntryEntity> BuildQuery(IWebHookRepository repository, WebhookFeedSearchCriteria searchCriteria)
        {
            var query = repository.WebHookFeedEntries;

            if (!string.IsNullOrWhiteSpace(searchCriteria.SearchPhrase))
            {
                query = query.Where(x => x.EventId.Contains(searchCriteria.SearchPhrase));
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

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(WebhookFeedSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] {
                            new SortInfo { SortColumn = nameof(WebHookFeedEntryEntity.CreatedDate), SortDirection = SortDirection.Descending }
                        };
            }
            return sortInfos;
        }
    }
}
