using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides model for web api call
    /// </summary>
    public class WebApiCall<TResult>
    {
        private readonly HttpRequestMessage _request;
        private readonly IHttpRequestInvoker _requestInvoker;
        
        /// <summary>
        /// HTTP response
        /// </summary>
        public HttpResponseMessage Response { get; private set; }

        /// <summary>
        /// Result payload
        /// </summary>
        public TResult Result { get; private set; }

        internal IHttpMessagesListener HttpMessagesListener { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiCall"/>
        /// </summary>
        public WebApiCall(HttpRequestMessage request, IHttpRequestInvoker requestInvoker)
        {
            _request = request;
            _requestInvoker = requestInvoker;
        }

        /// <summary>
        /// Creates request clone. Also clones the content.
        /// </summary>
        public async Task<HttpRequestMessage> GetRequestClone()
        {
            return await HttpRequestMessageTools.Clone(_request);
        }

        /// <summary>
        /// Invokes HTTP call
        /// </summary>
        public async Task<TResult> Invoke()
        {
            return await Invoke(CancellationToken.None);
        }

        /// <summary>
        /// Invokes HTTP call
        /// </summary>
        public async Task<TResult> Invoke(CancellationToken cancellationToken)
        {
            Response = await _requestInvoker.Send(await GetRequestClone(), cancellationToken);

            HttpMessagesListener?.Notify(await GetRequestClone(), Response);

            if(typeof(TResult) != typeof(CodeResult)) 
                await RightStatusChecker.Check(GetRequestClone, Response);

            Result = (TResult) await HttpContentTools.ExtractResult(Response, typeof(TResult));
            return Result;
        }
    }

    /// <summary>
    /// Provides model for web api call
    /// </summary>
    public class WebApiCall
    {
        private readonly HttpRequestMessage _request;
        private readonly IHttpRequestInvoker _requestInvoker;

        /// <summary>
        /// HTTP response
        /// </summary>
        public HttpResponseMessage Response { get; private set; }

        internal IHttpMessagesListener HttpMessagesListener { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiCall"/>
        /// </summary>
        public WebApiCall(HttpRequestMessage request, IHttpRequestInvoker requestInvoker)
        {
            _requestInvoker = requestInvoker;
            _request = request;
        }

        /// <summary>
        /// Creates request clone. Also clones the content.
        /// </summary>
        public async Task<HttpRequestMessage> GetRequestClone()
        {
            return await HttpRequestMessageTools.Clone(_request);
        }

        /// <summary>
        /// Invokes HTTP call
        /// </summary>
        public async Task Invoke()
        {
            await Invoke(CancellationToken.None);
        }

        /// <summary>
        /// Invokes HTTP call
        /// </summary>
        public async Task Invoke(CancellationToken cancellationToken)
        {
            Response = await _requestInvoker.Send(await GetRequestClone(), cancellationToken);
            HttpMessagesListener?.Notify(await GetRequestClone(), Response);

            await RightStatusChecker.Check(GetRequestClone, Response);
        }
    }
}