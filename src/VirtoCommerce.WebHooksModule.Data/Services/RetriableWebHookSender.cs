using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;
using VirtoCommerce.WebHooksModule.Data.Utils;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    /// <summary>
    /// Provides an implementation of <see cref="IWebHookSender"/> for sending HTTP requests to
    /// registered <see cref="Webhook"/> instances with retry mechanism.
    /// </summary>
    public class RetriableWebHookSender : WebHookSenderBase
    {
        protected const string UnsuccessfulSendTemplate = "WebHook was sent unsuccessfully. Attempt number: {0}. Error: {1}.";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHookLogger _logger;
        private readonly IWebHookFeedService _webHookFeedService;
        private readonly ISettingsManager _settingsManager;
        private int? _retryCount;

        private bool _disposed;

        protected int RetryCount
        {
            get
            {
                if (_retryCount == null)
                {
                    _retryCount = _settingsManager.GetValue(ModuleConstants.Settings.General.SendRetryCount.Name, 3);
                }

                return (int)_retryCount;
            }
        }

        public RetriableWebHookSender(IWebHookLogger logger, IWebHookFeedService webHookFeedService, ISettingsManager settingsManager,
            IHttpClientFactory httpClientFactory) : base()
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _webHookFeedService = webHookFeedService;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                base.Dispose(disposing);
            }
        }

        public override Task<WebhookSendResponse> SendWebHookAsync(WebhookWorkItem webHookWorkItem)
        {
            return PerformSend(webHookWorkItem);
        }

        private async Task<WebhookSendResponse> PerformSend(WebhookWorkItem webHookWorkItem)
        {
            var result = new WebhookSendResponse();

            try
            {
                var request = CreateWebHookRequest(webHookWorkItem);
                var httpClient = _httpClientFactory.CreateClient("webhooks");

                var response = await httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                result = CreateSendResponse(response, responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new WebHookSendException(responseContent, webHookWorkItem.WebHook.Id, webHookWorkItem.EventId);
                }

                return result;
            }
            catch (WebHookSendException ex)
            {
                result.Error = GetErrorText(webHookWorkItem.FeedEntry?.AttemptCount + 1 ?? 0, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                return result;
            }
            finally
            {
                await HandleResponseAsync(webHookWorkItem, result);
            }
        }

        private WebhookSendResponse CreateSendResponse(HttpResponseMessage response, string responseString)
        {
            return new WebhookSendResponse()
            {
                StatusCode = (int)(response?.StatusCode ?? 0),
                ResponseParams = new WebhookHttpParams()
                {
                    Headers = response?.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value)) ?? new Dictionary<string, string>(),
                    Body = responseString
                },
                Error = response != null && response.IsSuccessStatusCode ? string.Empty: responseString,
                IsSuccessfull = response?.IsSuccessStatusCode ?? false
            };
        }

        protected virtual Task HandleResponseAsync(WebhookWorkItem webHookWorkItem, WebhookSendResponse webHookSendResponse)
        {
            if (webHookSendResponse.IsSuccessfull)
            {
                return HandleSuccessAsync(webHookWorkItem, webHookSendResponse);
            }
            else
            {
                return HandleErrorAsync(webHookWorkItem, webHookSendResponse);
            }
        }

        protected virtual async Task HandleSuccessAsync(WebhookWorkItem webHookWorkItem, WebhookSendResponse webHookSendResponse)
        {
            // Clear error records on this sending after success
            if (webHookWorkItem.FeedEntry?.RecordType == (int)WebhookFeedEntryType.Error && !string.IsNullOrEmpty(webHookWorkItem.FeedEntry.Id))
            {
                await _webHookFeedService.DeleteByIdsAsync(new[] { webHookWorkItem.FeedEntry.Id });
            }

            // Create new record (not using and updating existing one!) for proper logging, as all Success entries are accumulated in one record in our current logging policy.
            var feedEntry = WebHookFeedUtils.CreateSuccessEntry(webHookWorkItem, webHookSendResponse);

            await _logger.LogAsync(feedEntry);
            webHookWorkItem.FeedEntry = feedEntry;
        }

        protected virtual async Task HandleErrorAsync(WebhookWorkItem webHookWorkItem, WebhookSendResponse webHookSendResponse)
        {
            var feedEntry = GetOrCreateFeedEntry(webHookWorkItem, webHookSendResponse);

            feedEntry.RecordType = (int)WebhookFeedEntryType.Error;
            feedEntry.Status = webHookSendResponse?.StatusCode ?? webHookWorkItem.FeedEntry.Status;

            await _logger.LogAsync(feedEntry);
            webHookWorkItem.FeedEntry = feedEntry;
        }

        /// <summary>
        /// Creates <see cref="WebhookFeedEntry"/> from <see cref="webHookWorkItem"/>. If it already has <see cref="WebhookFeedEntry"/>, inreases AttemptCount.
        /// </summary>
        /// <param name="webHookWorkItem"></param>
        /// <param name="webHookSendResponse"></param>
        /// <returns></returns>
        protected virtual WebhookFeedEntry GetOrCreateFeedEntry(WebhookWorkItem webHookWorkItem, WebhookSendResponse webHookSendResponse)
        {
            var feedEntry = webHookWorkItem.FeedEntry;

            if (feedEntry == null)
            {
                feedEntry = WebHookFeedUtils.CreateErrorEntry(webHookWorkItem, webHookSendResponse);
            }
            else
            {
                feedEntry.AttemptCount++;
            }

            return feedEntry;
        }

        protected virtual string GetErrorText(int attemptCount, string errorDetail)
        {
            return string.Format(UnsuccessfulSendTemplate, attemptCount, errorDetail);
        }
    }
}
