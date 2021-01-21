using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class WebHookLogger : IWebHookLogger
    {
        private readonly IWebHookFeedService _webHookFeedService;
        private readonly IWebHookFeedSearchService _webHookFeedSearchService;

        public WebHookLogger(IWebHookFeedService webHookFeedService, IWebHookFeedSearchService webHookFeedSearchService)
        {
            _webHookFeedService = webHookFeedService;
            _webHookFeedSearchService = webHookFeedSearchService;
        }

        public virtual async Task<WebhookFeedEntry> LogAsync(WebhookFeedEntry feedEntry)
        {
            if (feedEntry == null)
            {
                throw new ArgumentNullException(nameof(feedEntry));
            }

            WebhookFeedEntry result = null;

            using (await AsyncLock.GetLockByKey(CacheKey.With(typeof(WebhookFeedEntry), feedEntry.WebHookId)).LockAsync())
            {
                switch (feedEntry.RecordType)
                {
                    case (int)WebhookFeedEntryType.Success:
                        result = await LogSuccessAsync(feedEntry);
                        break;
                    case (int)WebhookFeedEntryType.Error:
                        result = await LogErrorAsync(feedEntry);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported {nameof(WebhookFeedEntry.RecordType)}: {feedEntry.RecordType}.");
                }
            }

            return result;
        }

        /// <summary>
        /// Logs Success webHook notifications by incrementing AttemptCount.
        /// </summary>
        /// <param name="feedEntry">Entry with the event data.</param>
        /// <returns>Saved feed entry.</returns>
        protected async Task<WebhookFeedEntry> LogSuccessAsync(WebhookFeedEntry feedEntry)
        {
            var criteria = new WebhookFeedSearchCriteria()
            {
                RecordTypes = new[] { (int)WebhookFeedEntryType.Success },
                WebHookIds = new[] { feedEntry.WebHookId },
                Skip = 0,
                Take = 1,
            };
            var feedEntrySearchResult = await _webHookFeedSearchService.SearchAsync(criteria);
            var feedEntryToSave = feedEntrySearchResult.Results.FirstOrDefault();

            if (feedEntryToSave == null)
            {
                feedEntryToSave = feedEntry;
            }

            feedEntryToSave.AttemptCount++;

            await _webHookFeedService.SaveChangesAsync(new[] { feedEntryToSave });

            return feedEntryToSave;
        }

        /// <summary>
        /// Logs Error in webHook notifications by saving log record.
        /// </summary>
        /// <param name="feedEntry"></param>
        /// <returns>Saved feed entry.</returns>
        protected virtual async Task<WebhookFeedEntry> LogErrorAsync(WebhookFeedEntry feedEntry)
        {
            if (feedEntry.IsTransient())
            {
                await _webHookFeedService.SaveChangesAsync(new[] { feedEntry });
            }
            else
            {
                await _webHookFeedService.UpdateCountAttemps(new[] { feedEntry });
            }

            return feedEntry;
        }

    }
}
