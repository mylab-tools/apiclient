using System.IO;
using System.Web;
using System.Web.Http;

namespace IntegrationTest.Server.Controllers.Rest
{
    public class BinaryResourceController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            var files = HttpContext.Current.Request.Files;
            
            using (var sr = new BinaryReader(files[0].InputStream))
            {
                return Ok(sr.ReadBytes(files[0].ContentLength));
            }
        }
    }
}
