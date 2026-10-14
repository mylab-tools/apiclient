using System.Net.Http;
using System.Net.Http.Headers;
using MyLab.ApiClient.JsonSerialization;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class JsonHttpContentFactory : IHttpContentFactory
{
    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        var serializer = settings?.JsonSerializer ?? NewtonJsonSerializer.Default;
        var serialized = serializer.Serialize(source);


        var content = new StringContent(serialized)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
        };

        //To calculate `content-length` header
        var len = content.Headers.ContentLength.GetValueOrDefault();

        return content;
    }
}