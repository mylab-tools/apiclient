using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.ResponseProcessing;

class CallDetailsFactory(IContentDeserializerProvider deserializerProvider, HttpMessageDumper? dumper)
{   
    public async Task<CallDetails> CreateAsync(HttpRequestMessage req, HttpResponseMessage resp)
    {
        var reqDump = dumper != null 
            ? await dumper.DumpAsync(req)
            : null;

        var respDump = dumper != null
            ? await dumper.DumpAsync(resp)
            : null;

        return new CallDetails(deserializerProvider)
        {
            IsOK = IsOK(resp.StatusCode),
            RequestDump = reqDump,
            ResponseDump = respDump,
            RequestMessage = req,
            ResponseMessage = resp,
            StatusCode = resp.StatusCode
        };
    }
    
    bool IsOK(HttpStatusCode statusCode) => (int)statusCode >= 200 && (int)statusCode < 300;
}