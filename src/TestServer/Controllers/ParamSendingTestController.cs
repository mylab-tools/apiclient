using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestServer.Models;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("param-sending")]
    public class ParamSendingTestController : ControllerBase
    {
        [HttpPost("echo/multipart")]
        public IActionResult EchoMultipart()
        {
            return Ok(Request.Form["part1"] + Request.Form["part2"]);
        }

        [HttpPost("echo/query")]
        public IActionResult EchoQuery([FromQuery]string msg)
        {
            return Ok(msg);
        }

        [HttpPost("echo/{msg}/path")]
        public IActionResult EchoPath([FromRoute]string msg)
        {
            return Ok(msg);
        }

        [HttpPost("echo/header")]
        public IActionResult EchoHeader([FromHeader(Name = "Message")]string msg, [FromHeader(Name = "Message2")] string msg2)
        {
            return Ok(msg + msg2);
        }

        [HttpPost("echo/body/obj/xml")]
        public async Task<IActionResult> EchoObjXml()
        {
            var reader = new StreamReader(Request.Body);
            var strContent = await reader.ReadToEndAsync();

            var ser = new XmlSerializer(typeof(TestModel));
            using var strReader = new StringReader(strContent);
            var rdr = XmlReader.Create(strReader, new XmlReaderSettings
            {
                IgnoreProcessingInstructions = true
            });
            var model = (TestModel)ser.Deserialize(rdr);

            return Ok(model.TestValue);
        }

        [HttpPost("echo/body/obj/json")]
        public IActionResult EchoObjJson([FromBody]TestModel model)
        {
            return Ok(model.TestValue);
        }

        [HttpPost("echo/body/form")]
        public IActionResult EchoForm([FromForm]TestModel model)
        {
            return Ok(model.TestValue);
        }

        [HttpPost("echo/body/form-with-name")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult EchoForm(IFormCollection form)
        {
            return Ok(form["test_value"].FirstOrDefault());
        }

        [HttpPost("echo/body/text")]
        public async Task<IActionResult> EchoForm()
        {
            var rdr = new StreamReader(Request.Body);
            return Ok(await rdr.ReadToEndAsync());
        }

        [HttpPost("echo/body/bin")]
        public async Task<IActionResult> EchoBin()
        {
            var rdr = new StreamReader(Request.Body, Encoding.UTF8);
            return Ok(await rdr.ReadToEndAsync());
        }
    }
}