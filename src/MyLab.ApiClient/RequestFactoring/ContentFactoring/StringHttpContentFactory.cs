using MyLab.ApiClient.Tools;
using System.Net.Http;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class StringHttpContentFactory : IHttpContentFactory
{
    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        return new StringContent(ObjectToStringConverter.ToString(source));
    }
}