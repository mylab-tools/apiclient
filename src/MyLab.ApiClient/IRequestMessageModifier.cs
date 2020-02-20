using System.Net.Http;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Integrates into request message preparing pipeline
    /// </summary>
    public interface IRequestMessageModifier
    {
        /// <summary>
        /// Modifies <see cref="HttpRequestMessage"/>
        /// </summary>
        void Modify(HttpRequestMessage msg);
    }
}