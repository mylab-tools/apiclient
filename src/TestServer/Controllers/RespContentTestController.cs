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
            return Ok("<Root><TestValue>foo</TestValue></Root>");
        }

        [HttpGet("data/json")]
        public IActionResult GetJsonData()
        {
            return Ok("{\"TestValue\":\"foo\"}");
        }

        [HttpGet("data/enumerable")]
        public IEnumerable<string> GetEnumerable()
        {
            return new[] {"foo", "bar"};
        }
    }
}