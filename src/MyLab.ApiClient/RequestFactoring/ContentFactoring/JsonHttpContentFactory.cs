using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class JsonHttpContentFactory : IHttpContentFactory
{
    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        var serialized = settings?.JsonSettings != null
            ? JsonConvert.SerializeObject(source, settings.JsonSettings)
            : JsonConvert.SerializeObject(source);

        var content = new StringContent(serialized)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
        };

        //To calculate `content-length` header
        var len = content.Headers.ContentLength.GetValueOrDefault();

        return content;
    }
}