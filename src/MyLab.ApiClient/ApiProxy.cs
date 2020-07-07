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
        internal ApiRequestFactory ApiRequestFactory;
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

            ApiRequestFactory = new ApiRequestFactory(contractType, httpClientProvider);
            _callTaskFactory = new GenericCallTaskFactory(ApiRequestFactory);
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var retType = targetMethod.ReturnType;

            if (retType == typeof(Task))
                return ApiRequestFactory.Create(targetMethod, args).GetResult(CancellationToken.None);
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
