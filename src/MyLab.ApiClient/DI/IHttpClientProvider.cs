using System.Net.Http;

namespace MyLab.ApiClient.DI
{
    interface IHttpClientProvider
    {
        HttpClient Provide();
    }
}
