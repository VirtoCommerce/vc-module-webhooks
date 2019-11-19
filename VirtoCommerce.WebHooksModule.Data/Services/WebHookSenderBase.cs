using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    /// <summary>
    /// Provides a base implementation of <see cref="IWebHookSender"/> that defines the default format
    /// for HTTP requests sent as WebHooks.
    /// Based on https://github.com/aspnet/AspNetWebHooks/blob/master/src/Microsoft.AspNet.WebHooks.Custom/WebHooks/WebHookSender.cs
    /// </summary>
    public abstract class WebHookSenderBase : IWebHookSender, IDisposable
    {
        private const string EventIdKey = "EventId";
        private const string AttemptKey = "Attempt";
        private const string EventBodyKey = "EventBody";

        protected const string InvalidHeaderTemplate = "Could not add header field '{0}' to the WebHook request for WebHook ID '{1}'.";
        protected const string UnsuccessfulResponseTemplate = "WebHook was sent unsuccessfully. Response status code: {0}";

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHookSenderBase"/> class.
        /// </summary>
        protected WebHookSenderBase(IWebHookLogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the current <see cref="ILogger"/> instance.
        /// </summary>
        protected IWebHookLogger Logger { get; }

        /// <inheritdoc />
        public abstract Task<WebHookResponse> SendWebHookAsync(WebHookWorkItem webHookWorkItem);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <b>false</b> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    // Dispose any resources
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="HttpRequestMessage"/> containing the headers and body by given <paramref name="workItem"/>.
        /// </summary>
        /// <param name="workItem">A <see cref="WebHookWorkItem"/> representing the <see cref="WebHook"/> to be sent.</param>
        /// <returns>A filled in <see cref="HttpRequestMessage"/>.</returns>
        //[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Request is disposed by caller.")]
        protected virtual HttpRequestMessage CreateWebHookRequest(WebHookWorkItem workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            var webHook = workItem.WebHook;

            // Create WebHook request
            var request = new HttpRequestMessage(HttpMethod.Post, webHook.Url);

            // Fills in request body and headers

            CreateWebHookRequestBody(workItem, request);

            foreach (var kvp in webHook.RequestParams.Headers)
            {
                if (!request.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value) && !request.Content.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value))
                {
                    var errorMessage = string.Format(CultureInfo.CurrentCulture, InvalidHeaderTemplate, kvp.Key, webHook.Id);

                    throw new WebHookSendException(errorMessage, webHook.Id, workItem.EventId);
                }
            }

            return request;
        }

        private static void CreateWebHookRequestBody(WebHookWorkItem workItem, HttpRequestMessage request)
        {
            var body = new Dictionary<string, object>
            {
                // Set properties from work item
                [EventIdKey] = workItem.EventId,
                [AttemptKey] = workItem.Offset + 1,
                [EventBodyKey] = workItem.WebHook.RequestParams.Body,
            };

            var serializedBody = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(serializedBody, Encoding.UTF8, "application/json");
        }
    }
}
