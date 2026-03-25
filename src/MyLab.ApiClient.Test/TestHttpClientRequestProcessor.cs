using MyLab.ApiClient.Usage;
using System.Text;
using MyLab.ApiClient.Tools;
using Xunit;

namespace MyLab.ApiClient.Test
{
    class TestHttpClientRequestProcessor(IRequestProcessor innerProc, Type contractType, ITestOutputHelper? output) : IRequestProcessor
    {
        readonly HttpMessageDumper _dumper = new();

        public async Task<HttpResponseMessage> ProcessRequestAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var resp = await innerProc.ProcessRequestAsync(request, ct);

            if (output != null)
            {
                await WriteToOutputAsync(request, resp, contractType);
            }
            
            return resp;
        }

        async Task WriteToOutputAsync(HttpRequestMessage req, HttpResponseMessage resp,Type contractType)
        {
            var reqDump = await _dumper.DumpAsync(req);
            var respDump = await _dumper.DumpAsync(resp);

            var sb = new StringBuilder();
            
            Append(sb, "");
            Append(sb, $"===== REQUEST BEGIN ({contractType.Name}) =====");
            Append(sb, "");
            Append(sb, reqDump);
            Append(sb, "");
            Append(sb, "===== REQUEST END =====");
            Append(sb, "");
            Append(sb, $"===== RESPONSE BEGIN ({contractType.Name}) =====");
            Append(sb, "");
            Append(sb, respDump);
            Append(sb, "");
            Append(sb, "===== RESPONSE END =====");
            
            output!.WriteLine(sb.ToString()); 
        }

        static void Append(StringBuilder sb, string str)
        {
            sb.Append(str + "\r\n");
        }
    }
}
