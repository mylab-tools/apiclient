using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class HttpMessageDumper
    {
        public const int DefaultMexBodySize = 10 * 1024;
        public const string ContentIsTooLargeText = "... [content is too large to dump]";

        public int MaxRequestBodySize { get; set; } = DefaultMexBodySize;
        public int MaxResponseBodySize { get; set; } = DefaultMexBodySize;

        public async Task<string> Dump(HttpRequestMessage msg)
        {
            var b = new StringBuilder();
            b.AppendLine($"{msg.Method} {msg.RequestUri}");
            b.AppendLine();

            await Dump(b, msg.Headers, msg.Content, MaxRequestBodySize);

            return b.ToString();
        }

        public async Task<string> Dump(HttpResponseMessage msg)
        {
            var b = new StringBuilder();

            b.AppendLine($"{(int)msg.StatusCode} {msg.ReasonPhrase}");
            b.AppendLine();

            try
            {
                await Dump(b, msg.Headers, msg.Content, MaxResponseBodySize);
            }
            catch (Exception e)
            {
                b.AppendLine("Unexpected dump error: " + e);
            }

            return b.ToString();
        }

        public async Task<string> Dump(
            StringBuilder dumpBuilder, 
            HttpHeaders msgHeaders, 
            HttpContent content,
            int bodyLimit)
        {
            var rha = msgHeaders.ToArray();
            if (rha.Length != 0)
            {
                foreach (var header in rha)
                {
                    var hvs = header.Value.Select(
                        v => string.IsNullOrWhiteSpace(v)
                            ? "<empty>"
                            : v );
                    dumpBuilder.AppendLine(
                        $"{header.Key}: {string.Join(", ", hvs)}");
                }
            }

            if (content != null)
            {
                var cha = content.Headers.ToArray();
                if (cha.Length != 0)
                {
                    foreach (var header in cha)
                        dumpBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
                
                var strm = await content.ReadAsStreamAsync();

                if (strm.CanSeek)
                {
                    var buff = new byte[bodyLimit];
                    strm.Seek(0, SeekOrigin.Begin);

                    int readCount;
                    var contentBuilder = new StringBuilder();

                    while (
                        (readCount = await strm.ReadAsync(buff, 0, buff.Length)) != 0 &&
                        contentBuilder.Length < bodyLimit
                        )
                    {
                        var encodingFromRequest =
                            content.Headers?.ContentEncoding?.FirstOrDefault();
                        var encoding = encodingFromRequest != null
                            ? Encoding.GetEncoding(encodingFromRequest)
                            : Encoding.UTF8;

                        var strContent = encoding.GetString(buff, 0, readCount);

                        contentBuilder.AppendLine(strContent);
                    }

                    if (contentBuilder.Length != 0)
                    {
                        dumpBuilder.AppendLine();

                        bool contentIsTooLarge = contentBuilder.Length > bodyLimit;

                        var normContent = contentIsTooLarge
                            ? contentBuilder.Remove(bodyLimit, contentBuilder.Length-bodyLimit)
                            : contentBuilder;

                        dumpBuilder.AppendLine(normContent.ToString());
                        if (contentIsTooLarge)
                            dumpBuilder.AppendLine(ContentIsTooLargeText);
                    }
                }
            }

            return dumpBuilder.ToString();
        }
    }
}
