using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains tools to create dump from http request and response
    /// </summary>
    public class HttpMessageDumper
    {
        /// <summary>
        /// Default max body size for dumping
        /// </summary>
        public const int DefaultMaxBodySize = 10 * 1024;

        /// <summary>
        /// DumpAsync text when body is too large
        /// </summary>
        public const string ContentIsTooLargeText = "... [content is too large to dump]";

        /// <summary>
        /// Set to customize request body size which is maximum dor dumping
        /// </summary>
        public int MaxRequestBodySize { get; set; } = DefaultMaxBodySize;
        /// <summary>
        /// Set to customize response body size which is maximum dor dumping
        /// </summary>
        public int MaxResponseBodySize { get; set; } = DefaultMaxBodySize;

        /// <summary>
        /// Dumps a request
        /// </summary>
        public async Task<string> DumpAsync(HttpRequestMessage msg)
        {
            var b = new StringBuilder();
            AppendLine(b, $"{msg.Method} {msg.RequestUri}");
            AppendLine(b, "");

            await DumpAsync(b, msg.Headers, msg.Content, MaxRequestBodySize);

            return b.ToString();
        }

        /// <summary>
        /// Dumps a response
        /// </summary>
        public async Task<string> DumpAsync(HttpResponseMessage msg)
        {
            var b = new StringBuilder();

            AppendLine(b, $"{(int)msg.StatusCode} {msg.ReasonPhrase}");
            AppendLine(b, "");

            try
            {
                await DumpAsync(b, msg.Headers, msg.Content, MaxResponseBodySize);
            }
            catch (Exception e)
            {
                AppendLine(b, "Unexpected dump error: " + e);
            }

            return b.ToString();
        }

        async Task DumpAsync(
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
                    AppendLine(dumpBuilder, 
                        $"{header.Key}: {string.Join(", ", hvs)}");
                }
            }

            if (content != null)
            {
                var cha = content.Headers.ToArray();
                if (cha.Length != 0)
                {
                    foreach (var header in cha)
                        AppendLine(dumpBuilder, $"{header.Key}: {string.Join(", ", header.Value)}");
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

                        AppendLine(contentBuilder, strContent);
                    }

                    if (contentBuilder.Length != 0)
                    {
                        AppendLine(dumpBuilder, "");

                        bool contentIsTooLarge = contentBuilder.Length > bodyLimit;

                        var normContent = contentIsTooLarge
                            ? contentBuilder.Remove(bodyLimit, contentBuilder.Length-bodyLimit)
                            : contentBuilder;

                        AppendLine(dumpBuilder, normContent.ToString());
                        if (contentIsTooLarge)
                            AppendLine(dumpBuilder, ContentIsTooLargeText);
                    }
                }
            }
        }

        void AppendLine(StringBuilder sb, string str) => sb.Append(str + "\r\n");
    }
}
