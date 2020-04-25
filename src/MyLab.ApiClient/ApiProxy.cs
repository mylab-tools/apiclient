using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    public class ApiProxy<TContract> : DispatchProxy
    {
        private ApiRequestFactory _apiRequestFactory;
        private GenericCallTaskFactory _callTaskFactory;

        public static TContract Create(IHttpClientProvider httpClientProvider)
        {
            if (httpClientProvider == null) throw new ArgumentNullException(nameof(httpClientProvider));

            object proxy = Create<TContract, ApiProxy<TContract>>();
            
            ((ApiProxy<TContract>)proxy).Initialize(httpClientProvider);

            return (TContract)proxy;
        }

        private void Initialize(IHttpClientProvider httpClientProvider)
        {
            var contractType = typeof(TContract);

            _apiRequestFactory = new ApiRequestFactory(contractType, httpClientProvider);
            _callTaskFactory = new GenericCallTaskFactory(_apiRequestFactory);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var retType = targetMethod.ReturnType;

            if (retType == typeof(Task))
                return _apiRequestFactory.Create(targetMethod, args).GetResult(CancellationToken.None);
            else
            {

                if (!retType.IsGenericType || retType.GetGenericTypeDefinition() != typeof(Task<>))
                    throw new ApiClientException($"Wrong method return type '{retType.FullName}'");
                var gRetType = retType.GetGenericArguments().First();

                return _callTaskFactory.Create(targetMethod, args, gRetType);
            }
        }
    }
}
