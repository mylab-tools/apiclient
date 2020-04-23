using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("resp-info")]
    public class RespInfoTestController : ControllerBase
    {
        [HttpGet("400")]
        public IActionResult Get400()
        {
            return BadRequest("This is a message");
        }

        [HttpGet("200")]
        public IActionResult Get200()
        {
            return Ok();
        }
    }
}