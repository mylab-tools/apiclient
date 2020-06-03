﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace TestServer.Controllers
{
    [ApiController]
    [Route("resp-content")]
    public class RespContentTestController : ControllerBase
    {
        [HttpGet("data/xml")]
        public IActionResult GetXmlData()
        {
            return Content("<Root><TestValue>foo</TestValue></Root>", "application/xml");
        }

        [HttpGet("data/json")]
        public IActionResult GetJsonData()
        {
            return Content("{\"TestValue\":\"foo\"}", "application/json");
        }

        [HttpGet("data/enumerable")]
        public IEnumerable<string> GetEnumerable()
        {
            return new[] {"foo", "bar"};
        }

        [HttpGet("data/int")]
        public IActionResult GetInt()
        {
            return Ok(10);
        }

        [HttpGet("data/float")]
        public IActionResult GetFloat()
        {
            return Ok(10.1);
        }

        [HttpGet("data/bin-json")]
        public IActionResult GetBinJson()
        {
            var bin = Encoding.UTF8.GetBytes("foo");
            var json = JsonConvert.SerializeObject(bin);

            return Content(json, "application/json");
        }

        [HttpGet("data/bin-octet-stream")]
        public IActionResult GetBinOctetStream()
        {
            var bin = Encoding.UTF8.GetBytes("foo");
            var memStream = new MemoryStream(bin);
            
            return new FileStreamResult(memStream, "application/octet-stream");
        }
    }
}