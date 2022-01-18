using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestServer.Models;

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

        [HttpGet("obj")]
        public async Task<IActionResult> GetObj()
        {
            using var rdr = new StreamReader(Request.Body);
            
            return Ok(await rdr.ReadToEndAsync());
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