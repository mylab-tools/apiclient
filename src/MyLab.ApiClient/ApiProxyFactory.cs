using System;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    public class ApiProxy<TContract> : DispatchProxy
    {
        private ApiRequestFactory<TContract> _apiRequestFactory;

        public static TContract Create(IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));

            object proxy = Create<TContract, ApiProxy<TContract>>();
            
            ((ApiProxy<TContract>)proxy).Initialize(httpClientFactory);

            return (TContract)proxy;
        }

        private void Initialize(IHttpClientFactory httpClientFactory)
        {
            var contractType = typeof(TContract);
            var desc = ServiceDescription.Create(contractType);

            var configKey = contractType.GetCustomAttribute<ApiAttribute>()?.Key;
            var client = httpClientFactory.CreateClient(configKey);

            _apiRequestFactory = new ApiRequestFactory<TContract>(desc, client);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return null;
        }
    }
}
