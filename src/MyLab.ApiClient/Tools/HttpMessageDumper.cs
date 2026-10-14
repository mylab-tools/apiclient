using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.ApiClient.Tools;

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
    /// Placeholder written instead of sensitive header values
    /// </summary>
    public const string SensitiveInfoPlaceholder = "[sensitive-info]";

    static readonly HashSet<string> SensitiveHeaderSet = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Proxy-Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key",
        "Api-Key",
        "X-Auth-Token",
        "X-Access-Token",
        "X-CSRF-Token"
    };

    /// <summary>
    /// Header names whose values are masked in dumps unless <see cref="ShowSensitiveInfo"/> is <c>true</c>.
    /// Comparison is case-insensitive.
    /// </summary>
    public static IReadOnlyCollection<string> SensitiveHeaders => SensitiveHeaderSet;

    /// <summary>
    /// Set to customize request body size which is maximum dor dumping
    /// </summary>
    public int MaxRequestBodySize { get; set; } = DefaultMaxBodySize;
    /// <summary>
    /// Set to customize response body size which is maximum dor dumping
    /// </summary>
    public int MaxResponseBodySize { get; set; } = DefaultMaxBodySize;

    /// <summary>
    /// When <c>true</c>, sensitive header values are written as-is.
    /// When <c>false</c> (default), values of headers from <see cref="SensitiveHeaders"/>
    /// are replaced with <see cref="SensitiveInfoPlaceholder"/>.
    /// </summary>
    public bool ShowSensitiveInfo { get; set; }

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
        HttpContent? content,
        int bodyLimit)
    {
        var rha = msgHeaders.ToArray();
        if (rha.Length != 0)
        {
            foreach (var header in rha)
            {
                AppendLine(dumpBuilder, FormatHeader(header.Key, header.Value));
            }
        }

        if (content is { Headers.ContentLength: not null })
        {
            var cha = content.Headers.ToArray();
            if (cha.Length != 0)
            {
                foreach (var header in cha)
                    AppendLine(dumpBuilder, FormatHeader(header.Key, header.Value));
            }

            var strm = await content.ReadAsStreamAsync();

            if (strm.CanSeek)
            {
                var buff = new byte[bodyLimit];
                strm.Seek(0, SeekOrigin.Begin);

                int readCount;

                if ((readCount = await strm.ReadAsync(buff, 0, buff.Length)) != 0)
                {
                    var encodingFromRequest =
                        content.Headers.ContentEncoding.FirstOrDefault();
                    var encoding = encodingFromRequest != null
                        ? Encoding.GetEncoding(encodingFromRequest)
                        : Encoding.UTF8;

                    var strContent = encoding.GetString(buff, 0, readCount);

                    dumpBuilder.Append(strContent);
                }

                bool contentIsTooLarge = content.Headers.ContentLength > bodyLimit;

                if (contentIsTooLarge)
                {
                    AppendLine(dumpBuilder, "");
                    dumpBuilder.Append(ContentIsTooLargeText);
                }
            }
        }
    }

    string FormatHeader(string name, IEnumerable<string> values)
    {
        if (!ShowSensitiveInfo && IsSensitiveHeader(name))
            return $"{name}: {SensitiveInfoPlaceholder}";

        var hvs = values.Select(
            v => string.IsNullOrWhiteSpace(v)
                ? "<empty>"
                : v);

        return $"{name}: {string.Join(", ", hvs)}";
    }

    static bool IsSensitiveHeader(string name) => SensitiveHeaderSet.Contains(name);

    void AppendLine(StringBuilder sb, string str) => sb.Append(str + "\r\n");
}
