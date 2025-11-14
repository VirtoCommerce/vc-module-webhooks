using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Data.Caching;
using VirtoCommerce.WebhooksModule.Data.Models;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class WebHookService : IWebHookService
    {
        private readonly Func<IWebHookRepository> _webHookRepositoryFactory;
        private readonly IWebHookFeedReader _feedReader;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public WebHookService(Func<IWebHookRepository> webHookRepositoryFactory, IWebHookFeedReader feedReader, IPlatformMemoryCache platformMemoryCache)
        {
            _webHookRepositoryFactory = webHookRepositoryFactory;
            _feedReader = feedReader;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<Webhook[]> GetByIdsAsync(string[] ids, string responseGroup = null)
        {
            var webhookResponseGroup = EnumUtility.SafeParse(responseGroup, WebhookResponseGroup.Full);

            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", ids));
            var result = await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(WebhookCacheRegion.CreateChangeToken());

                var result = new List<Webhook>();

                if (!ids.IsNullOrEmpty())
                {
                    using (var repository = _webHookRepositoryFactory())
                    {
                        var entities = await repository.GetWebHooksByIdsAsync(ids);

                        if (!entities.IsNullOrEmpty())
                        {
                            result.AddRange(entities.Select(x => x.ToModel(AbstractTypeFactory<Webhook>.TryCreateInstance())));
                        }
                    }
                }

                return result;
            });

            if (webhookResponseGroup.HasFlag(WebhookResponseGroup.WithFeed))
            {
                var webHookIds = result.Select(x => x.Id).ToArray();
                var webHookSuccessCounts = await _feedReader.GetSuccessCountsAsync(webHookIds);
                var webHookErrorCounts = await _feedReader.GetErrorCountsAsync(webHookIds);

                foreach (var webHook in result)
                {
                    webHook.SuccessCount = webHookSuccessCounts.FirstOrDefault(w => w.Key.EqualsIgnoreCase(webHook.Id)).Value;
                    webHook.ErrorCount = webHookErrorCounts.FirstOrDefault(w => w.Key.EqualsIgnoreCase(webHook.Id)).Value;
                }
            }

            return result.ToArray();
        }

        public async Task SaveChangesAsync(Webhook[] webHooks)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Webhook>>();

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
                        changedEntries.Add(new GenericChangedEntry<Webhook>(webHook, originalEntity.ToModel(new Webhook()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);

                        //Force set ModifiedDate property to mark a webHook changed. Special for update when only event list has changes and webHook table hasn't any changes
                        originalEntity.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<Webhook>(webHook, EntryState.Added));
                    }
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();

                ClearCache();
            }
        }

        public async Task DeleteByIdsAsync(string[] ids)
        {
            using (var repository = _webHookRepositoryFactory())
            {
                await repository.DeleteWebHooksByIdsAsync(ids);
                await repository.UnitOfWork.CommitAsync();

                ClearCache();
            }
        }


        protected virtual void ClearCache()
        {
            WebhookCacheRegion.ExpireRegion();
            WebhookSearchCacheRegion.ExpireRegion();
        }
    }
}
