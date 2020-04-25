using Microsoft.AspNetCore.Mvc;

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
    }
}