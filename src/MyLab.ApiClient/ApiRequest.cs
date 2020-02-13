using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class ApiRequestBase
    {
        public List<IRequestMessageModifier> RequestModifiers { get; }
            = new List<IRequestMessageModifier>();

        protected string BaseUrl { get; }
        protected MethodDescription MethodDescription { get; }
        protected IHttpClientProvider HttpClientProvider { get; }
        protected IReadOnlyList<IParameterApplier> ParamAppliers { get; }

        protected ApiRequestBase(
            string baseUrl,
            MethodDescription methodDescription, 
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider)
        {
            if (paramAppliers == null) throw new ArgumentNullException(nameof(paramAppliers));
            BaseUrl = baseUrl;
            MethodDescription = methodDescription ?? throw new ArgumentNullException(nameof(methodDescription));
            HttpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
            ParamAppliers = paramAppliers.ToList().AsReadOnly();
        }

        protected async Task<HttpResponseMessage> SendRequestAsync(CancellationToken cancellationToken)
        {
            var cl = HttpClientProvider.Provide();

            var reqMsg = new HttpRequestMessage
            {
                Method = MethodDescription.HttpMethod,
            };


            ApplyInputParameters(reqMsg);

            ApplyModifiers(reqMsg);

            var response = await cl.SendAsync(reqMsg, cancellationToken);

            CheckResponseCode(response);

            return response;
        }

        private void ApplyInputParameters(HttpRequestMessage reqMsg)
        {
            
        }

        private void ApplyModifiers(HttpRequestMessage reqMsg)
        {
            foreach (var requestModifier in RequestModifiers)
            {
                try
                {
                    requestModifier?.Modify(reqMsg);
                }
                catch (Exception e)
                {
                    throw new ApiClientException("An error occured while request modifying", e);
                }
            }
        }

        private void CheckResponseCode(HttpResponseMessage response)
        {
            throw new NotImplementedException();
        }
    }

    class ApiRequestWithReturnValue<TValue> : ApiRequestBase, IApiRequest<TValue>
    {
        public ApiRequestWithReturnValue(
            string baseUrl, 
            MethodDescription methodDescription, 
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider) 
            : base(baseUrl, methodDescription, paramAppliers, httpClientProvider)
        {
        }
        
        public TValue Call()
        {
            throw new System.NotImplementedException();
        }
    }
    
    class ApiRequestWithoutReturnValue : ApiRequestBase, IApiRequest
    {
        public ApiRequestWithoutReturnValue(
            string baseUrl, 
            MethodDescription methodDescription, 
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider) 
            : base(baseUrl, methodDescription, paramAppliers, httpClientProvider)
        {
        }

        public void Call()
        {
            throw new System.NotImplementedException();
        }
    }
}