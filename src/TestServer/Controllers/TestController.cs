using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Identity;
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

        [HttpPost("ping/body/obj/xml")]
        public async Task<IActionResult> PingObjXml()
        {
            var reader = new StreamReader(Request.Body);
            var strContent = await reader.ReadToEndAsync();

            var ser = new XmlSerializer(typeof(TestModel));
            using var strReader = new StringReader(strContent);
            var rdr = XmlReader.Create(strReader, new XmlReaderSettings
            {
                IgnoreProcessingInstructions = true
            });
            var model =(TestModel) ser.Deserialize(rdr);

            return Ok(model.TestValue);
        }

        [HttpPost("ping/body/obj/json")]
        public IActionResult PingObjJson([FromBody]TestModel model)
        {
            return Ok(model.TestValue);
        }

        [HttpPost("ping/body/form")]
        public IActionResult PingForm([FromForm]TestModel model)
        {
            return Ok(model.TestValue);
        }

        [HttpPost("ping/body/text")]
        public async Task<IActionResult> PingForm()
        {
            var rdr = new StreamReader(Request.Body);
            return Ok(await rdr.ReadToEndAsync());
        }

        [HttpPost("ping/body/bin")]
        public async Task<IActionResult> PingBin()
        {
            var rdr = new StreamReader(Request.Body, Encoding.UTF8);
            return Ok(await rdr.ReadToEndAsync());
        }
    }
}
