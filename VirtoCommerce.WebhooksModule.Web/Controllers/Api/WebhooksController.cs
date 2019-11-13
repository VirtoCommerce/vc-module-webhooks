using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.Platform.Core.Web.Security;
using VirtoCommerce.WebhooksModule.Core;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Web.Controllers.Api
{
    [RoutePrefix("api/webhooks")]
    public class WebhooksController : ApiController
    {
        private readonly IWebhookSearchService _webhookSearchService;
        private readonly IWebhookFeedSearchService _webhookFeedSearchService;
        private readonly IWebhookService _webhookService;

        public WebhooksController(IWebhookSearchService webhookSearchService,
            IWebhookFeedSearchService webhookFeedSearchService,
            IWebhookService webhookService)
        {
            _webhookSearchService = webhookSearchService;
            _webhookFeedSearchService = webhookFeedSearchService;
            _webhookService = webhookService;
        }

        // GET: api/webhooks/:id
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(Webhook))]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public IHttpActionResult GetWebhookById(string id)
        {
            var result = _webhookService.GetByIds(new[] { id });

            return Ok(result?.FirstOrDefault());
        }

        /// <summary>
        /// Searches webhooks by certain criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(WebhookSearchResult))]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public WebhookSearchResult Search(WebhookSearchCriteria criteria)
        {
            var result = _webhookSearchService.Search(criteria);

            return result;
        }

        /// <summary>
        /// Searches webhook logs by certain criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("feed/search")]
        [ResponseType(typeof(WebhookFeedSearchResult))]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.ReadFeed)]
        public WebhookFeedSearchResult SearchWebhookFeed(WebhookFeedSearchCriteria criteria)
        {
            var result = _webhookFeedSearchService.Search(criteria);

            return result;
        }

        /// <summary>
        /// Creates or updates the webhooks.
        /// </summary>
        /// <param name="webhooks">Webhooks to save.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Update)]
        public IHttpActionResult SaveWebhook(Webhook[] webhooks)
        {
            _webhookService.SaveChanges(webhooks);

            return Ok();
        }

        /// <summary>
        /// Deletes webhooks by ids.
        /// </summary>
        /// <param name="ids">Webhook ids to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Update)]
        public IHttpActionResult DeleteWebhookds(string[] ids)
        {
            _webhookService.DeleteByIds(ids);

            return Ok();
        }

        /// <summary>
        /// Sends request with given params to webhook and returns result
        /// </summary>
        /// <param name="testRequest">Request params.</param>
        /// <returns>Result of sent request.</returns>
        [HttpPost]
        [Route("send")]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public IHttpActionResult Run(WebhookTestRequest testRequest)
        {
            throw new NotImplementedException();
        }
    }
}
