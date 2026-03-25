using System.Net;
using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace TestServer.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("status")]
    public async Task<IActionResult> GetStatusCodeAsync()
    {
        using var rdr = new StreamReader(Request.Body);
        var statusCodeStr = await rdr.ReadToEndAsync();
        var statusCode = Enum.Parse<HttpStatusCode>(statusCodeStr);
            
        return StatusCode((int)statusCode);
    }

    [HttpPost("dto/xml")]
    public IActionResult GetXmlDto(TestDto dto)
    {
        var s = new XmlSerializer(typeof(TestDto));
        using var mem = new MemoryStream();
        s.Serialize(mem, dto);
        var strContent = Encoding.UTF8.GetString(mem.ToArray());
        return Content(strContent, "application/xml");
    }

    [HttpPost("dto/json")]
    public IActionResult GetJsonDto(TestDto dto)
    {
        return new JsonResult(dto);
    }
}