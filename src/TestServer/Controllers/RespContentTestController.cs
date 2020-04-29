using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("resp-content")]
    public class RespContentTestController : ControllerBase
    {
        [HttpGet("data/xml")]
        public IActionResult GetXmlData()
        {
            return Content("<Root><TestValue>foo</TestValue></Root>", "application/xml");
        }

        [HttpGet("data/json")]
        public IActionResult GetJsonData()
        {
            return Content("{\"TestValue\":\"foo\"}", "application/json");
        }

        [HttpGet("data/enumerable")]
        public IEnumerable<string> GetEnumerable()
        {
            return new[] {"foo", "bar"};
        }

        [HttpGet("data/int")]
        public IActionResult GetInt()
        {
            return Ok(10);
        }

        [HttpGet("data/float")]
        public IActionResult GetFloat()
        {
            return Ok(10.1);
        }
    }
}