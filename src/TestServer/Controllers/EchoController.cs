using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("echo")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromBody]string msg)
        {
            return Ok(msg);
        }

        [HttpGet("empty")]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpHead("empty")]
        public IActionResult GetHead()
        {
            return Ok();
        }
    }
}