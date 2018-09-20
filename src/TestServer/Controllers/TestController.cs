using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> RootGet()
        {
            return Ok("RootGet");
        }

        [HttpGet("foo/bar")]
        public ActionResult<string> GetWithPath()
        {
            return Ok("GetWithPath");
        }

        [HttpGet("foo/bar/{id}")]
        public ActionResult<string> GetWithParametrizedPath(string id)
        {
            return Ok(id);
        }

        //// GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
