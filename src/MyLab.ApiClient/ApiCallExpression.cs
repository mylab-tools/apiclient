using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace MyLab.ApiClient
{
    class ApiCallExpressionBase
    {
        protected string BaseUrl { get; }
        protected MethodDescription MethodDescription { get; }
        protected IHttpClientFactory HttpClientFactory { get; }
        protected IReadOnlyList<ApiCallParameter> Parameters { get; }

        protected ApiCallExpressionBase(
            string baseUrl,
            MethodDescription methodDescription, 
            IEnumerable<ApiCallParameter> parameters,
            IHttpClientFactory httpClientFactory)
        {
            BaseUrl = baseUrl;
            MethodDescription = methodDescription;
            HttpClientFactory = httpClientFactory;
            Parameters = parameters.ToList().AsReadOnly();
        }
    }

    class ApiCallExpressionWithReturnValue<TValue> : ApiCallExpressionBase, IApiCallExpression<TValue>
    {
        public ApiCallExpressionWithReturnValue(
            string baseUrl, 
            MethodDescription methodDescription, 
            IEnumerable<ApiCallParameter> parameters,
            IHttpClientFactory httpClientFactory) 
            : base(baseUrl, methodDescription, parameters, httpClientFactory)
        {
        }
        
        public TValue Call()
        {
            throw new System.NotImplementedException();
        }
    }
    
    class ApiCallExpressionWithoutReturnValue : ApiCallExpressionBase, IApiCallExpression
    {
        public ApiCallExpressionWithoutReturnValue(
            string baseUrl, 
            MethodDescription methodDescription, 
            IEnumerable<ApiCallParameter> parameters,
            IHttpClientFactory httpClientFactory) 
            : base(baseUrl, methodDescription, parameters, httpClientFactory)
        {
        }

        public void Call()
        {
            throw new System.NotImplementedException();
        }
    }
}