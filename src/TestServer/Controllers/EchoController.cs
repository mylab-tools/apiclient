using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("echo")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string msg)
        {
            return Ok(msg);
        }
    }
}