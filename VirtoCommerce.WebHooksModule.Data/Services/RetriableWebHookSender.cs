using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;
using VirtoCommerce.WebHooksModule.Data.Utils;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    /// <summary>
    /// Provides an implementation of <see cref="IWebHookSender"/> for sending HTTP requests to
    /// registered <see cref="WebHook"/> instances with retry mechanism.
    /// </summary>
    public class RetriableWebHookSender : WebHookSenderBase
    {
        protected const string UnsuccessfulSendTemplate = "WebHook was sent unsuccessfully. Attempt number: {0}. Error: {1}.";

        private readonly HttpClient _httpClient;
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
                    _retryCount = _settingsManager.GetValue("Webhooks.General.SendRetryCount", 3);
                }

                return (int)_retryCount;
            }
        }

        public RetriableWebHookSender(IWebHookLogger logger, IWebHookFeedService webHookFeedService, ISettingsManager settingsManager) : base()
        {
            _httpClient = new HttpClient();
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
                if (disposing)
                {
                    try
                    {
                        // Cancel any outstanding HTTP requests
                        if (_httpClient != null)
                        {
                            _httpClient.CancelPendingRequests();
                            _httpClient.Dispose();
                        }

                    }
                    catch
                    {
                        // No need in throwing
                    }
                }
                base.Dispose(disposing);
            }
        }

        public override async Task<WebHookSendResponse> SendWebHookAsync(WebHookWorkItem webHookWorkItem)
        {
            WebHookSendResponse result = null;

            try
            {
                // Retry in the following intervals (in minutes): 1, 2, 4, …, 2^(RetryCount-1)
                //CodeReview: Need to create retry policy is more selective and do retry only for specific kind of errors that can be classified as transient fault.
                //Instead of this you would call multiple times a endpoint that can be misconfigured.
                var policy = Policy
                    .HandleResult<WebHookSendResponse>(x => !x.IsSuccessfull)
                    .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMinutes(Math.Pow(2, retryAttempt - 1)));

                result = await policy.ExecuteAsync(async () => await PerformSend(webHookWorkItem));

            }
            catch (Exception ex)
            {
                var feedEntry = webHookWorkItem?.FeedEntry ?? WebHookFeedUtils.CreateErrorEntry(webHookWorkItem, result, ex.Message);

                feedEntry.Error = ex.Message;
                _logger.Log(feedEntry);
            }


            return result;
        }

        private async Task<WebHookSendResponse> PerformSend(WebHookWorkItem webHookWorkItem)
        {
            WebHookSendResponse result;
            HttpResponseMessage response = null;

            try
            {
                var request = CreateWebHookRequest(webHookWorkItem);
                //CodeReview: Use named or typed HttpClientFactory instead
                response = await _httpClient.SendAsync(request);

                result = await CreateSendResponse(response);

                if (response.IsSuccessStatusCode)
                {
                    HandleSuccess(webHookWorkItem, result);
                }
                else
                {
                    var errorMessage = GetErrorText(webHookWorkItem.FeedEntry?.AttemptCount + 1 ?? 0, $"Response staus code does not indicate success: {(int)response.StatusCode}");

                    result.Error = errorMessage;
                    HandleError(webHookWorkItem, result, errorMessage);
                }
            }
            catch (Exception ex)
            {

                var errorMessage = GetErrorText(webHookWorkItem.FeedEntry?.AttemptCount + 1 ?? 0, ex.Message);

                result = await CreateSendResponse(response);
                result.Error = errorMessage;
                HandleError(webHookWorkItem, result, errorMessage);
            }

            return result;
        }

        private async Task<WebHookSendResponse> CreateSendResponse(HttpResponseMessage response)
        {
            return new WebHookSendResponse()
            {
                StatusCode = (int)(response?.StatusCode ?? 0),
                ResponseParams = await CreateResponseParams(response),
            };
        }

        protected virtual void HandleSuccess(WebHookWorkItem webHookWorkItem, WebHookSendResponse webHookSendResponse)
        {
            // Clear error records on this sending after success
            if (webHookWorkItem.FeedEntry?.RecordType == (int)WebHookFeedEntryType.Error && !string.IsNullOrEmpty(webHookWorkItem.FeedEntry.Id))
            {
                _webHookFeedService.DeleteByIds(new[] { webHookWorkItem.FeedEntry.Id });
            }

            // Create new record (not using and updating existing one!) for proper logging, as all Success entries are accumulated in one record in our current logging policy.
            var feedEntry = WebHookFeedUtils.CreateSuccessEntry(webHookWorkItem, webHookSendResponse);

            _logger.Log(feedEntry);
            webHookWorkItem.FeedEntry = feedEntry;
        }

        protected virtual void HandleError(WebHookWorkItem webHookWorkItem, WebHookSendResponse webHookSendResponse, string errorMessage)
        {
            var feedEntry = GetOrCreateFeedEntry(webHookWorkItem, webHookSendResponse, errorMessage);

            feedEntry.RecordType = (int)WebHookFeedEntryType.Error;
            feedEntry.Error = errorMessage;
            feedEntry.Status = webHookSendResponse?.StatusCode ?? webHookWorkItem.FeedEntry.Status;

            _logger.Log(feedEntry);
            webHookWorkItem.FeedEntry = feedEntry;
        }

        /// <summary>
        /// Creates <see cref="WebHookFeedEntry"/> from <see cref="webHookWorkItem"/>. If it already has <see cref="WebHookFeedEntry"/>, inreases AttemptCount.
        /// </summary>
        /// <param name="webHookWorkItem"></param>
        /// <param name="webHookSendResponse"></param>
        /// <returns></returns>
        protected virtual WebHookFeedEntry GetOrCreateFeedEntry(WebHookWorkItem webHookWorkItem, WebHookSendResponse webHookSendResponse, string errorMessage)
        {
            var feedEntry = webHookWorkItem.FeedEntry;

            if (feedEntry == null)
            {
                feedEntry = WebHookFeedUtils.CreateErrorEntry(webHookWorkItem, webHookSendResponse, errorMessage);
            }
            else
            {
                feedEntry.AttemptCount++;
            }

            return feedEntry;
        }

        protected virtual async Task<WebHookHttpParams> CreateResponseParams(HttpResponseMessage response)
        {
            var responseString = await response?.Content.ReadAsStringAsync();

            return new WebHookHttpParams()
            {
                Headers = response?.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value)) ?? new Dictionary<string, string>(),
                Body = !string.IsNullOrEmpty(responseString) ? JObject.Parse(responseString) : null,
            };
        }

        protected virtual string GetErrorText(int attemptCount, string errorDetail)
        {
            return string.Format(UnsuccessfulSendTemplate, attemptCount, errorDetail);
        }
    }
}
