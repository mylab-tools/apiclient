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
        private readonly IHttpRequestInvoker _requestInvoker;

        /// <summary>
        /// HTTP request
        /// </summary>
        public HttpRequestMessage Request { get; }

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
            _requestInvoker = requestInvoker;
            Request = request;
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
            var requestClone = await HttpRequestMessageTools.Clone(Request);
            Response = await _requestInvoker.Send(Request, cancellationToken);
            HttpMessagesListener?.Notify(requestClone, Response);

            RightStatusChecker.Check(Request, Response);

            Result = (TResult) HttpContentTools.ExtractResult(Response, typeof(TResult));
            return await Task.FromResult(Result);
        }
    }

    /// <summary>
    /// Provides model for web api call
    /// </summary>
    public class WebApiCall
    {
        private readonly IHttpRequestInvoker _requestInvoker;

        /// <summary>
        /// HTTP request
        /// </summary>
        public HttpRequestMessage Request { get; }

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
            Request = request;
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
            var requestClone = await HttpRequestMessageTools.Clone(Request);
            Response = await _requestInvoker.Send(Request, cancellationToken);
            HttpMessagesListener?.Notify(requestClone, Response);

            RightStatusChecker.Check(Request, Response);
        }
    }
}