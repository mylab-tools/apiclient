using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("test")]
    public class TestController : ControllerBase
    {
        [HttpGet("400")]
        public IActionResult Get400()
        {
            var r = BadRequest("This is a message");
            return r;
        }
    }
}
