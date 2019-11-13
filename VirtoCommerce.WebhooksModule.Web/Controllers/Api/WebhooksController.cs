using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.Domain.Commerce.Model.Search;
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

        public WebhooksController(IWebhookSearchService webhookSearchService)
        {
            _webhookSearchService = webhookSearchService;
        }

        // GET: api/webhooks/:id
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(WebWebhook))]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public IHttpActionResult GetWebhookById()
        {
            return Ok(new WebWebhook()
            {
                Id = "test",
                IsActive = false,
                Name = "test",
                Url = "https://myLAUrl",
                RaisedEventCount = 100500,
                EventErrorsCount = 500
            });
        }

        /// <summary>
        /// Searches webhooks by certain criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(GenericSearchResult<WebWebhook>))]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public GenericSearchResult<WebWebhook> Search(WebhookSearchCriteria criteria)
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
        [ResponseType(typeof(GenericSearchResult<WebWebhookFeed>))]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.ReadFeed)]
        public GenericSearchResult<WebWebhookFeed> SearchWebhookLigs(WebhookSearchCriteria criteria)
        {
            var result = _webhookSearchService.SearchFeed(criteria);
            return result;
        }

        /// <summary>
        /// Creates thumbnail task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Update)]
        public IHttpActionResult Create(WebWebhook webhook)
        {
            return Ok();
        }

        [HttpPost]
        [Route("send")]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public IHttpActionResult Run()
        {
            return Ok();
        }
    }
}
