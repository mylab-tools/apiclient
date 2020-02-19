using Microsoft.AspNetCore.Mvc;
using TestServer.Models;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        [HttpGet("400")]
        public IActionResult Get400()
        {
            return BadRequest("This is a message");
        }

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

        [HttpPost("ping/query")]
        public IActionResult PingQuery([FromQuery]string msg)
        {
            return Ok(msg);
        }

        [HttpPost("ping/{msg}/path")]
        public IActionResult PingPath([FromRoute]string msg)
        {
            return Ok(msg);
        }

        [HttpPost("ping/header")]
        public IActionResult PingHeader([FromHeader(Name = "Message")]string msg)
        {
            return Ok(msg);
        }

        [HttpPost("ping/body/obj")]
        public IActionResult PingObj([FromBody]TestModel model)
        {
            return Ok(model.TestValue);
        }

        [HttpPost("ping/body/form")]
        public IActionResult PingForm([FromBody]TestModel model)
        {
            return Ok(model.TestValue);
        }


    }
}
