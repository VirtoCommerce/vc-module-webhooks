using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Data.Services
{
    /// <summary>
    /// Provides an implementation of <see cref="IWebHookSender"/> for sending HTTP requests to
    /// registered <see cref="WebHook"/> instances with retry mechanism.
    /// Based on https://github.com/aspnet/AspNetWebHooks/blob/master/src/Microsoft.AspNet.WebHooks.Custom/WebHooks/DataFlowWebHookSender.cs
    /// </summary>
    public class RetriableWebHookSender : WebHookSenderBase
    {
        private readonly HttpClient _httpClient;

        public RetriableWebHookSender() : base()
        {
            _httpClient = new HttpClient();
        }

        public override async Task<WebHookResponse> SendWebHookAsync(WebHookWorkItem webHookWorkItem)
        {
            WebHookResponse result;
            HttpResponseMessage response = null;

            try
            {
                var request = CreateWebHookRequest(webHookWorkItem);
                response = await _httpClient.SendAsync(request);

                result = new WebHookResponse()
                {
                    StatusCode = (int)response.StatusCode,
                    ResponseParams = await CreateResponseParams(response),
                };

                if (!response.IsSuccessStatusCode)
                {
                    result.Error = string.Format(UnsuccessfulResponseTemplate, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                result = new WebHookResponse()
                {
                    StatusCode = (int)(response?.StatusCode ?? 0),
                    Error = ex.Message,
                    ResponseParams = await CreateResponseParams(response)
                };
            }

            return result;
        }

        private static async Task<WebHookHttpParams> CreateResponseParams(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync();

            return new WebHookHttpParams()
            {
                Headers = response.Headers.ToDictionary(x => x.Key, x => string.Join(";", x.Value)),
                Body = JObject.Parse(responseString),
            };
        }
    }
}
