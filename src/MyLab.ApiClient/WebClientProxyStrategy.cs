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

        public IHttpMessagesListener HttpMessagesListener { get; set; }

        public WebClientProxyStrategy(IHttpRequestInvoker httpRequestInvoker)
        {
            _httpRequestInvoker = httpRequestInvoker;
        }
        
        public WebApiCall GetCall(
            MethodInfo method, 
            ApiClientDescription description, 
            object[] args)
        {
            var methodDesc = description.GetMethod(method.MetadataToken);
            var urlBuilder = new UrlBuilder(description.RelPath, methodDesc);
            var url = urlBuilder.Build(args);

            var msg = new HttpRequestMessage(methodDesc.HttpMethod, url);

            var msgContent = CreateContent(methodDesc, args);

            msg.Content = msgContent;

            return new WebApiCall(msg, _httpRequestInvoker)
            {
                HttpMessagesListener = HttpMessagesListener
            };
        }

        public WebApiCall<TResult> GetCall<TResult>(
            MethodInfo method,
            ApiClientDescription description,
            object[] args)
        {
            var methodDesc = description.GetMethod(method.MetadataToken);
            var urlBuilder = new UrlBuilder(description.RelPath, methodDesc);
            var url = urlBuilder.Build(args);

            var msg = new HttpRequestMessage(methodDesc.HttpMethod, url);

            var msgContent = CreateContent(methodDesc, args);

            msg.Content = msgContent;

            return new WebApiCall<TResult>(msg, _httpRequestInvoker)
            {
                HttpMessagesListener = HttpMessagesListener
            };
        }

        private HttpContent CreateContent(MethodDescription methodDesc, object[] args)
        {
            var bodyParam = methodDesc.Params.FirstOrDefault(p => p.Place == ApiParamPlace.Body);
            if (bodyParam == null) return null;

            return HttpContentTools.CreateContent(args[bodyParam.Position], bodyParam.MimeType);
        }
    }
}