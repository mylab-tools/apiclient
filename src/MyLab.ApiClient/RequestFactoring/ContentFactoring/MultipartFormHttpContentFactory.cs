using System.Net.Http;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class MultipartFormHttpContentFactory : IHttpContentFactory
{
    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        var httpContent = new MultipartFormDataContent();
        
        if (source is IMultipartContentParameter parameter)
        {
            parameter.AddParts(httpContent);
        }

        return httpContent;
    }
}