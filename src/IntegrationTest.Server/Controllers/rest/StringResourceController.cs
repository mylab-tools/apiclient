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
        public IHttpActionResult Post([FromBody]string str)
        {
            return Ok(str);
        }
    }
}
