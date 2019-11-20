using System;
using System.Collections.Concurrent;
using System.Linq;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    public class WebHookLogger : IWebHookLogger
    {
        private readonly IWebHookFeedService _webHookFeedService;
        private readonly IWebHookFeedSearchService _webHookFeedSearchService;

        private readonly ConcurrentDictionary<string, object> syncRoots = new ConcurrentDictionary<string, object>();

        public WebHookLogger(IWebHookFeedService webHookFeedService, IWebHookFeedSearchService webHookFeedSearchService)
        {
            _webHookFeedService = webHookFeedService;
            _webHookFeedSearchService = webHookFeedSearchService;
        }

        public virtual WebHookFeedEntry Log(WebHookFeedEntry feedEntry)
        {
            if (feedEntry == null)
            {
                throw new ArgumentNullException(nameof(feedEntry));
            }

            var syncRoot = syncRoots.GetOrAdd(feedEntry.WebHookId, (x) => new object());
            WebHookFeedEntry result = null;

            lock (syncRoot)
            {
                switch (feedEntry.RecordType)
                {
                    case (int)WebHookFeedEntryType.Success:
                        result = LogSuccess(feedEntry);
                        break;
                    case (int)WebHookFeedEntryType.Error:
                        result = LogError(feedEntry);
                        break;
                    default:
                        throw new ArgumentException($"Unsupported {nameof(WebHookFeedEntry.RecordType)}: {feedEntry.RecordType}.");
                }
            }

            return result;
        }

        /// <summary>
        /// Logs Success webHook notifications by incrementing AttemptCount.
        /// </summary>
        /// <param name="feedEntry">Entry with the event data.</param>
        /// <returns>Saved feed entry.</returns>
        protected WebHookFeedEntry LogSuccess(WebHookFeedEntry feedEntry)
        {
            var criteria = new WebHookFeedSearchCriteria()
            {
                RecordTypes = new[] { (int)WebHookFeedEntryType.Success },
                WebHookIds = new[] { feedEntry.WebHookId },
                Skip = 0,
                Take = 1,
            };
            var feedEntrySearchResult = _webHookFeedSearchService.Search(criteria);
            var feedEntryToSave = feedEntrySearchResult.Results.FirstOrDefault();

            if (feedEntryToSave == null)
            {
                feedEntryToSave = feedEntry;
            }

            feedEntryToSave.AttemptCount++;

            _webHookFeedService.SaveChanges(new[] { feedEntryToSave });

            return feedEntryToSave;
        }

        /// <summary>
        /// Logs Error in webHook notifications by saving log record.
        /// </summary>
        /// <param name="feedEntry"></param>
        /// <returns>Saved feed entry.</returns>
        protected virtual WebHookFeedEntry LogError(WebHookFeedEntry feedEntry)
        {
            _webHookFeedService.SaveChanges(new[] { feedEntry });

            return feedEntry;
        }

    }
}
