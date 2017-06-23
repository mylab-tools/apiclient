using System.IO;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Xml.Serialization;

namespace IntegrationTest.Server.Controllers.api
{
    public class StringServiceController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ConcatStrings([FromBody] string main, string add1, string add2)
        {
            return Ok(main + add1 + add2);
        }

        [HttpPost]
        public IHttpActionResult GetJsonBack([FromBody]DataObject dataObject)
        {
            return Json(dataObject);
        }

        [HttpPost]
        public IHttpActionResult GetXmlBack()
        {
            var resp = Request.Content.ReadAsByteArrayAsync().Result;
            var s = new XmlSerializer(typeof(DataObject));

            using (var mem = new MemoryStream(resp))
            {
                return Ok(s.Deserialize(mem));
            }

            //return Ok(Encoding.UTF8.GetString(resp));
        }
    }

    public class DataObject
    {
        public string Value { get; set; }
    }
}
