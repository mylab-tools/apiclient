using System.Net.Http;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Declares http message listener
    /// </summary>
    public interface IHttpMessagesListener
    {
        /// <summary>
        /// Call to notify
        /// </summary>
        void Notify(HttpRequestMessage request, HttpResponseMessage response);
    }
}