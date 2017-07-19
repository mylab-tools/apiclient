using System.Web.Http;

namespace IntegrationTest.Server.Controllers.Rest
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
