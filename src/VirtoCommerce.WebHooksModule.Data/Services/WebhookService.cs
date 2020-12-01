using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.WebhooksModule.Data.Caching;
using VirtoCommerce.WebhooksModule.Data.Models;
using VirtoCommerce.WebhooksModule.Data.Repositories;
using VirtoCommerce.WebHooksModule.Core.Models;
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
                
        public async Task<WebHook[]> GetByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), nameof(GetByIdsAsync), string.Join("-", ids));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(WebhookCacheRegion.CreateChangeToken());

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
            });
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
        }
    }
}
