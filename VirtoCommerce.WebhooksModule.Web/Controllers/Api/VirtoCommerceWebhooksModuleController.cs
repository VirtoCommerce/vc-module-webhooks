using System.Web.Http;
using VirtoCommerce.WebhooksModule.Core;
using VirtoCommerce.Platform.Core.Web.Security;

namespace VirtoCommerce.WebhooksModule.Web.Controllers.Api
{
    [RoutePrefix("api/VirtoCommerceWebhooksModule")]
    public class VirtoCommerceWebhooksModuleController : ApiController
    {
        // GET: api/VirtoCommerceWebhooksModule
        [HttpGet]
        [Route("")]
        [CheckPermission(Permission = ModuleConstants.Security.Permissions.Read)]
        public IHttpActionResult Get()
        {
            return Ok(new { result = "Hello world!" });
        }
    }
}
