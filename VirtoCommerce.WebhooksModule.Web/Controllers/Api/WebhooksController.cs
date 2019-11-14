using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.Platform.Core.Web.Security;
using VirtoCommerce.WebhooksModule.Core;
using VirtoCommerce.WebhooksModule.Core.Models;
using VirtoCommerce.WebhooksModule.Core.Services;

namespace VirtoCommerce.WebhooksModule.Web.Controllers.Api
{
	[RoutePrefix("api/webhooks")]
	public class WebHooksController : ApiController
	{
		private readonly IWebHookSearchService _webHookSearchService;
		private readonly IWebHookFeedSearchService _webHookFeedSearchService;
		private readonly IWebHookService _webHookService;
		private readonly IWebHookManager _webHookManager;

		public WebHooksController(IWebHookSearchService webHookSearchService,
			IWebHookFeedSearchService webHookFeedSearchService,
			IWebHookService webHookService,
			IWebHookManager webHookManager)
		{
			_webHookSearchService = webHookSearchService;
			_webHookFeedSearchService = webHookFeedSearchService;
			_webHookService = webHookService;
			_webHookManager = webHookManager;
		}

		// GET: api/webhooks/:id
		[HttpGet]
		[Route("{id}")]
		[ResponseType(typeof(WebHook))]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
		public IHttpActionResult GetWebhookById(string id)
		{
			var result = _webHookService.GetByIds(new[] { id });

			return Ok(result?.FirstOrDefault());
		}

		/// <summary>
		/// Searches webhooks by certain criteria
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("search")]
		[ResponseType(typeof(WebHookSearchResult))]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
		public WebHookSearchResult Search(WebHookSearchCriteria criteria)
		{
			var result = _webHookSearchService.Search(criteria);

			return result;
		}

		/// <summary>
		/// Searches webhook logs by certain criteria
		/// </summary>
		/// <param name="criteria"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("feed/search")]
		[ResponseType(typeof(WebHookFeedSearchResult))]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.ReadFeed)]
		public WebHookFeedSearchResult SearchWebhookFeed(WebHookFeedSearchCriteria criteria)
		{
			var result = _webHookFeedSearchService.Search(criteria);

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
		public IHttpActionResult SaveWebhook(WebHook[] webhooks)
		{
			_webHookService.SaveChanges(webhooks);

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
			_webHookService.DeleteByIds(ids);

			return Ok();
		}

		/// <summary>
		/// Sends request with given params to webhook and returns result
		/// </summary>
		/// <param name="testRequest">Request params.</param>
		/// <returns>Result of sent request.</returns>
		[HttpPost]
		[Route("send")]
		[ResponseType(typeof(WebHookResponse))]
		[CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
		public async Task<IHttpActionResult> Run(WebHook webHook)
		{
			var result = await _webHookManager.VerifyWebHookAsync(webHook);

			return Ok(result);
		}
	}
}
