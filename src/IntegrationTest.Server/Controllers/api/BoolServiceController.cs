using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegrationTest.Server.Controllers.Api
{
    public class BoolServiceController : ApiController
    {
        public IHttpActionResult GetTrue()
        {
            return Ok(true);
        }
    }
}
