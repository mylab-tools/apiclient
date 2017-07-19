using System.Web.Http;

namespace IntegrationTest.Server.Controllers.Rest
{
    public class StringResourceController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Get(string str)
        {
            return Ok(str);
        }

        [HttpPost]
        public IHttpActionResult Post(string str)
        {
            return Ok(str);
        }
    }
}
