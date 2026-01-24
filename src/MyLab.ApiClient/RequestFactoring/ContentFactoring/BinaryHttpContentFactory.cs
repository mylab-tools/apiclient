using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class BinaryHttpContentFactory : IHttpContentFactory
{
    readonly string? _mimeType;

    public BinaryHttpContentFactory()
    {

    }

    public BinaryHttpContentFactory(string mimeType)
    {
        _mimeType = mimeType;
    }

    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is byte[] binSource)
        {
            var resContent = new ByteArrayContent(binSource);
            resContent.Headers.ContentType = new MediaTypeHeaderValue(_mimeType ?? "application/octet-stream");

            return resContent;
        }
        else
        {
            throw new NotSupportedException($"Only '{typeof(byte[]).FullName}' supported as binary argument");
        }
    }
}