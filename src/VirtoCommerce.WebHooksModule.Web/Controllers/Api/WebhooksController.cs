using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core;
using VirtoCommerce.WebHooksModule.Core.Models;
using VirtoCommerce.WebHooksModule.Core.Services;

namespace VirtoCommerce.WebHooksModule.Web.Controllers.Api
{
    [Route("api/webhooks")]
    [Authorize]
    public class WebHooksController : Controller
    {
        private readonly IWebHookSearchService _webHookSearchService;
        private readonly IWebHookFeedSearchService _webHookFeedSearchService;
        private readonly IWebHookService _webHookService;
        private readonly IWebHookManager _webHookManager;
        private readonly IRegisteredEventStore _registeredEventStore;
        private readonly IWebHookFeedService _webHookFeedService;
        private readonly IWebHookFeedReader _webHookFeedReader;

        public WebHooksController(IWebHookSearchService webHookSearchService,
            IWebHookFeedSearchService webHookFeedSearchService,
            IWebHookService webHookService,
            IWebHookManager webHookManager,
            IRegisteredEventStore registeredEventStore,
            IWebHookFeedService webHookFeedService,
            IWebHookFeedReader webHookFeedReader)
        {
            _webHookSearchService = webHookSearchService;
            _webHookFeedSearchService = webHookFeedSearchService;
            _webHookService = webHookService;
            _webHookManager = webHookManager;
            _registeredEventStore = registeredEventStore;
            _webHookFeedService = webHookFeedService;
            _webHookFeedReader = webHookFeedReader;
        }

        // GET: api/webhooks/:id
        /// <summary>
        /// Gets <see cref="Webhook"/> by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<Webhook>> GetWebhookById(string id)
        {
            var result = await _webHookService.GetByIdsAsync(new[] { id });

            return Ok(result?.FirstOrDefault());
        }

        /// <summary>
        /// Searches webhooks by certain criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<WebhookSearchResult>> Search([FromBody] WebhookSearchCriteria criteria)
        {
            var result = await _webHookSearchService.SearchAsync(criteria);
            var webHookIds = result.Results.Select(x => x.Id).ToArray();
            var webHookSuccessCounts = await _webHookFeedReader.GetSuccessCountsAsync(webHookIds);
            var webHookErrorCounts = await _webHookFeedReader.GetErrorCountsAsync(webHookIds);

            foreach (var webHook in result.Results)
            {
                webHook.SuccessCount = webHookSuccessCounts.FirstOrDefault(w => w.Key.EqualsInvariant(webHook.Id)).Value;
                webHook.ErrorCount = webHookErrorCounts.FirstOrDefault(w => w.Key.EqualsInvariant(webHook.Id)).Value;
            }

            return Ok(result);
        }

        /// <summary>
        /// Searches webhook logs by certain criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("feed/search")]
        [Authorize(ModuleConstants.Security.Permissions.ReadFeed)]
        public async Task<ActionResult<WebHookFeedSearchResult>> SearchWebhookFeed([FromBody] WebhookFeedSearchCriteria criteria)
        {
            var result = await _webHookFeedSearchService.SearchAsync(criteria);

            return Ok(result);
        }

        /// <summary>
        /// Delete webHookFeeds by ids.
        /// </summary>
        /// <param name="ids">WebHook Feeds ids to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("feed")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteWebHookFeeds([FromQuery] string[] ids)
        {
            await _webHookFeedService.DeleteByIdsAsync(ids);

            return Ok();
        }

        /// <summary>
        /// Creates or updates the webhooks.
        /// </summary>
        /// <param name="webhooks">Webhooks to save.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Update)]
        public async Task<ActionResult<Webhook[]>> SaveWebhooks([FromBody] Webhook[] webhooks)
        {
            await _webHookService.SaveChangesAsync(webhooks);

            return Ok(webhooks);
        }

        /// <summary>
        /// Deletes webhooks by ids.
        /// </summary>
        /// <param name="ids">Webhook ids to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [Authorize(ModuleConstants.Security.Permissions.Delete)]
        public async Task<ActionResult> DeleteWebHooks([FromQuery] string[] ids)
        {
            await _webHookService.DeleteByIdsAsync(ids);

            return Ok();
        }

        /// <summary>
        /// Sends request with given params to webhook and returns result
        /// </summary>
        /// <param name="webHook">Request params.</param>
        /// <returns>Result of sent request.</returns>
        [HttpPost]
        [Route("send")]
        public async Task<ActionResult<WebhookSendResponse>> Run([FromBody] Webhook webHook)
        {
            var result = await _webHookManager.VerifyWebHookAsync(webHook);

            return Ok(result);
        }

        // GET: api/webhooks/events
        /// <summary>
        /// Gets all registered events that could trigger webhook notification.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("events")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public ActionResult<RegisteredEvent[]> GetAllRegisteredEvents()
        {
            var result = _registeredEventStore.GetAllEvents();

            return Ok(result);
        }
    }
}
