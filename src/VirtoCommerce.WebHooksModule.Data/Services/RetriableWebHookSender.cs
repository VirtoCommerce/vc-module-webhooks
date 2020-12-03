using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly;
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
                    _retryCount = _settingsManager.GetValue(ModuleConstants.Settings.General.SendRetryCount.Name, 3);
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

        public override async Task<WebhookSendResponse> SendWebHookAsync(WebhookWorkItem webHookWorkItem)
        {
            WebhookSendResponse result = null;

            try
            {
                // Retry in the following intervals (in minutes): 1, 2, 4, …, 2^(RetryCount-1)
                var policy = Policy
                    .HandleResult<WebhookSendResponse>(x => !x.IsSuccessfull)
                    .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromMinutes(Math.Pow(2, retryAttempt - 1)));

                result = await policy.ExecuteAsync(async () => await PerformSend(webHookWorkItem));

            }
            catch (Exception ex)
            {
                var feedEntry = webHookWorkItem?.FeedEntry ?? WebHookFeedUtils.CreateErrorEntry(webHookWorkItem, result, ex.Message);

                feedEntry.Error = ex.Message;
                await _logger.LogAsync(feedEntry);
            }


            return result;
        }

        private async Task<WebhookSendResponse> PerformSend(WebhookWorkItem webHookWorkItem)
        {
            WebhookSendResponse result;
            HttpResponseMessage response = null;

            try
            {
                var request = CreateWebHookRequest(webHookWorkItem);
                response = await _httpClient.SendAsync(request);

                result = await CreateSendResponse(response);

                if (response.IsSuccessStatusCode)
                {
                    await HandleSuccessAsync(webHookWorkItem, result);
                }
                else
                {
                    var errorMessage = GetErrorText(webHookWorkItem.FeedEntry?.AttemptCount + 1 ?? 0, $"Response staus code does not indicate success: {(int)response.StatusCode}");

                    result.Error = errorMessage;
                    await HandleErrorAsync(webHookWorkItem, result, errorMessage);
                }
            }
            catch (Exception ex)
            {

                var errorMessage = GetErrorText(webHookWorkItem.FeedEntry?.AttemptCount + 1 ?? 0, ex.Message);

                result = await CreateSendResponse(response);
                result.Error = errorMessage;
                await HandleErrorAsync(webHookWorkItem, result, errorMessage);
            }

            return result;
        }

        private async Task<WebhookSendResponse> CreateSendResponse(HttpResponseMessage response)
        {
            return new WebhookSendResponse()
            {
                StatusCode = (int)(response?.StatusCode ?? 0),
                ResponseParams = await CreateResponseParams(response),
            };
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

        protected virtual async Task HandleErrorAsync(WebhookWorkItem webHookWorkItem, WebhookSendResponse webHookSendResponse, string errorMessage)
        {
            var feedEntry = GetOrCreateFeedEntry(webHookWorkItem, webHookSendResponse, errorMessage);

            feedEntry.RecordType = (int)WebhookFeedEntryType.Error;
            feedEntry.Error = errorMessage;
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
        protected virtual WebhookFeedEntry GetOrCreateFeedEntry(WebhookWorkItem webHookWorkItem, WebhookSendResponse webHookSendResponse, string errorMessage)
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

        protected virtual async Task<WebhookHttpParams> CreateResponseParams(HttpResponseMessage response)
        {
            var responseString = response != null ? await response.Content.ReadAsStringAsync() : string.Empty;

            return new WebhookHttpParams()
            {
                Headers = response?.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value)) ?? new Dictionary<string, string>(),
                Body = !string.IsNullOrEmpty(responseString) ? ParseResponseBody(responseString) : null,
            };
        }

        protected virtual JObject ParseResponseBody(string responseString)
        {
            JObject result;

            try
            {
                result = JObject.Parse(responseString);
            }
            catch (Exception)
            {
                result = new JObject(new JProperty("ParseError", "Content is not valid JSON object."));
            }

            return result;
        }

        protected virtual string GetErrorText(int attemptCount, string errorDetail)
        {
            return string.Format(UnsuccessfulSendTemplate, attemptCount, errorDetail);
        }
    }
}
