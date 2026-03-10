using MyLab.ApiClient.Options;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.ResponseProcessing;

class CallDetailsFactory
{
    readonly HttpMessageDumper _dumper;

    public CallDetailsFactory(ApiClientOptions options)
    {
        _dumper = options.Dumper ?? new HttpMessageDumper();
    }
    
    public async Task<CallDetails> CreateAsync(HttpRequestMessage req, HttpResponseMessage resp)
    {
        return new CallDetails
        {
            IsOK = IsOK(resp.StatusCode),
            RequestDump = await _dumper.DumpAsync(req),
            ResponseDump = await _dumper.DumpAsync(resp),
            RequestMessage = req,
            ResponseMessage = resp,
            StatusCode = resp.StatusCode
        };
    }
    
    bool IsOK(HttpStatusCode statusCode) => (int)statusCode >= 200 && (int)statusCode < 300;
}