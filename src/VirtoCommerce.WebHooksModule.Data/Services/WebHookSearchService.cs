using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.WebhooksModule.Data.Models;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Data.Services
{
    public class WebHookSearchService : IWebHookSearchService
    {
        private readonly IWebHookService _webHookService;
        private readonly Func<IWebHookRepository> _webHookRepositoryFactory;

        public WebHookSearchService(IWebHookService webHookService, Func<IWebHookRepository> webHookRepositoryFactory)
        {
            _webHookService = webHookService;
            _webHookRepositoryFactory = webHookRepositoryFactory;
        }

        public async Task<WebHookSearchResult> SearchAsync(WebHookSearchCriteria searchCriteria)
        {
            var result = new WebHookSearchResult();

            using (var repository = _webHookRepositoryFactory())
            {
                repository.DisableChangesTracking();

                var sortInfos = BuildSortExpression(searchCriteria);
                var query = BuildQuery(searchCriteria, repository);

                result.TotalCount = await query.CountAsync();

                if (searchCriteria.Take > 0 && result.TotalCount > 0)
                {
                    var webHookIds = query.OrderBySortInfos(sortInfos)
                                        .ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(searchCriteria.Skip)
                                        .Take(searchCriteria.Take)
                                        .ToArray();

                    result.Results = (await _webHookService.GetByIdsAsync(webHookIds)).OrderBy(x => Array.IndexOf(webHookIds, x.Id)).ToArray();
                }
            }

            return result;
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

        protected virtual IList<SortInfo> BuildSortExpression(WebHookSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = nameof(WebHookEntity.Name) }
                };
            }

            return sortInfos;
        }
    }
}
