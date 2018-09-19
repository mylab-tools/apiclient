using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class WebApiInvocation<TResult>
    {
        private readonly IHttpRequestInvoker _requestInvoker;

        public HttpRequestMessage Request { get; }

        public HttpResponseMessage Response { get; private set; }

        public TResult Result { get; private set; }

        public WebApiInvocation(HttpRequestMessage request, IHttpRequestInvoker requestInvoker)
        {
            _requestInvoker = requestInvoker;
            Request = request;
        }

        public async Task<TResult> Invoke(CancellationToken cancellationToken)
        {
            Response = await _requestInvoker.Send(Request, cancellationToken);
            RightStatusChecker.Check(Request, Response);

            Result = (TResult) HttpContentTools.ExtractResult(Response, typeof(TResult));
            return await Task.FromResult(Result);
        }
    }

    class WebApiInvocation
    {
        private readonly IHttpRequestInvoker _requestInvoker;

        public HttpRequestMessage Request { get; }

        public HttpResponseMessage Response { get; private set; }

        public WebApiInvocation(HttpRequestMessage request, IHttpRequestInvoker requestInvoker)
        {
            _requestInvoker = requestInvoker;
            Request = request;
        }

        public async Task Invoke(CancellationToken cancellationToken)
        {
            Response = await _requestInvoker.Send(Request, cancellationToken);
            RightStatusChecker.Check(Request, Response);
        }
    }
}