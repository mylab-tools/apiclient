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
    public async Task<IActionResult> GetXmlDto()
    {
        var s = new XmlSerializer(typeof(TestDto));

        var rdr = new StreamReader(Request.Body);
        var xmlStr = await rdr.ReadToEndAsync();
        using var txtReader = new StringReader(xmlStr);
        var dto = s.Deserialize(txtReader);
        
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