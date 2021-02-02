using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains detailed service call information with response
    /// </summary>
    public class CallDetails<T> : CallDetails
    {
        /// <summary>
        /// Expected response content
        /// </summary>
        public T ResponseContent { get; set; }
    }

    /// <summary>
    /// Contains detailed service call information 
    /// </summary>
    public class CallDetails
    {
        /// <summary>
        /// HTTP status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Gets true if status code is unexpected
        /// </summary>
        public bool IsUnexpectedStatusCode { get; set; }
        /// <summary>
        /// Text request dump
        /// </summary>
        public string RequestDump { get; set; }
        /// <summary>
        /// Text response dump
        /// </summary>
        public string ResponseDump { get; set; }
        /// <summary>
        /// Response object
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; set; }
        /// <summary>
        /// Request object
        /// </summary>
        public HttpRequestMessage RequestMessage { get; set; }

        /// <summary>
        /// Throws <see cref="ResponseCodeException"/> if <see cref="IsUnexpectedStatusCode"/> is true
        /// </summary>
        public async Task ThrowIfUnexpectedStatusCode()
        {
            if (!IsUnexpectedStatusCode)
                return;

            throw await ResponseCodeException.FromResponseMessage(ResponseMessage);
        }
    }
}