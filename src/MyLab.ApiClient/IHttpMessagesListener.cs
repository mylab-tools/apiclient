using System.Net.Http;

namespace MyLab.ApiClient
{
    interface IHttpMessagesListener
    {
        void Notify(HttpRequestMessage request, HttpResponseMessage response);
    }
}