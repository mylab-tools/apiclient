using System.Web.Http;

namespace IntegrationTest.Server.Controllers.api
{
    public class EmptyResourceController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
