using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class WebClientProxyStrategy : IClientProxyStrategy
    {
        private readonly IHttpRequestInvoker _httpRequestInvoker;

        public WebClientProxyStrategy(IHttpRequestInvoker httpRequestInvoker)
        {
            _httpRequestInvoker = httpRequestInvoker;
        }
        
        public WebApiInvocation GetInvocation(
            MethodInfo method, 
            ApiClientDescription description, 
            object[] args)
        {
            var methodDesc = description.GetMethod(method.MetadataToken);
            //var cancellationToken = GetCancellationToken(methodDesc, args);
            var urlBuilder = new UrlBuilder(description.RelPath, methodDesc);
            var url = urlBuilder.Build(args);

            var msg = new HttpRequestMessage(methodDesc.HttpMethod, url);

            var msgContent = CreateContent(methodDesc, args);

            msg.Content = msgContent;

            return new WebApiInvocation(msg, _httpRequestInvoker);
        }

        public WebApiInvocation<TResult> GetInvocation<TResult>(
            MethodInfo method,
            ApiClientDescription description,
            object[] args)
        {
            var methodDesc = description.GetMethod(method.MetadataToken);
            //var cancellationToken = GetCancellationToken(methodDesc, args);
            var urlBuilder = new UrlBuilder(description.RelPath, methodDesc);
            var url = urlBuilder.Build(args);

            var msg = new HttpRequestMessage(methodDesc.HttpMethod, url);

            var msgContent = CreateContent(methodDesc, args);

            msg.Content = msgContent;

            return new WebApiInvocation<TResult>(msg, _httpRequestInvoker);
        }

        private void CheckForRightStatus(HttpRequestMessage req, HttpResponseMessage resp)
        {
            if (!resp.IsSuccessStatusCode)
            {
                throw new WrongResponseException(req, resp);
            }
        }

        private HttpContent CreateContent(MethodDescription methodDesc, object[] args)
        {
            var bodyParam = methodDesc.Params.FirstOrDefault(p => p.Place == ApiParamPlace.Body);
            if (bodyParam == null) return null;

            return HttpContentTools.CreateContent(args[bodyParam.Position], bodyParam.MimeType);
        }

        CancellationToken GetCancellationToken(MethodDescription methodDescription, object[] args)
        {
            var cancelParam = methodDescription.Params.FirstOrDefault(p => p.IsCancellationToken);
            
            return cancelParam != null 
                ? (CancellationToken)args[cancelParam.Position]
                : CancellationToken.None;
        }
    }
}