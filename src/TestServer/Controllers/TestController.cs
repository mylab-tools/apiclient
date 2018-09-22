using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> RootGet()
        {
            return Ok("RootGet");
        }

        [HttpGet("foo/bar")]
        public ActionResult<string> GetWithPath()
        {
            return Ok("GetWithPath");
        }

        [HttpGet("foo/bar/{id}")]
        public ActionResult<string> GetWithParametrizedPath(string id)
        {
            return Ok(id);
        }

        [HttpGet("foo/bar/q")]
        public ActionResult<string> GetWithQuery(string id)
        {
            return Ok(id);
        }

        [HttpPost("post/bin")]
        public ActionResult<byte[]> PostBinary()
        {
            string strContent;
            using (var reader = new StreamReader(Request.Body))
                strContent = reader.ReadToEnd();

            var response = Encoding.UTF8.GetBytes(strContent);
            return Ok(response);
        }

        [HttpPost("post/xml-object")]
        public ContentResult PostXmlObj()
        {
            string strData;

            using (var reader = new StreamReader(Request.Body))
                strData = reader.ReadToEnd();

            return new ContentResult
            {
                Content = strData,
                ContentType = "application/xml",
                StatusCode = 200
            };
        }

        [HttpPost("post/json-object")]
        public ActionResult<TestObject> PostJsonObj([FromBody]TestObject arg)
        {
            return Ok(arg);
        }

        [HttpPost("post/form-object")]
        public ActionResult<TestObject> PostFormObj([FromForm]TestObject arg)
        {
            return Ok(arg);
        }
    }

    public class TestObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
