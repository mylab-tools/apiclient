using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Transparent proxy fro API contract
    /// </summary>
    public class ApiProxy<TContract> : DispatchProxy, IObservable<CallDetails>, IDisposable
    {
        private static readonly MethodInfo CallAndObserveGenericMethod =
            typeof(ApiProxy<TContract>)
                .GetMethods(BindingFlags.Instance | BindingFlags.
                    NonPublic)
                .First(m =>
                    m.Name == nameof(ApiProxy<TContract>.CallAndObserve) &&
                    m.IsGenericMethod);

        private static readonly MethodInfo CallWithDetailsAndObserveGenericMethod =
            typeof(ApiProxy<TContract>)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(m =>
                    m.Name == nameof(ApiProxy<TContract>.CallWithDetailsAndObserve) &&
                    m.IsGenericMethod);


        internal ApiRequestFactory ApiRequestFactory;
        private GenericCallTaskFactory _callTaskFactory;
        readonly ApiCallObservers _callObservers = new ApiCallObservers();

        /// <summary>
        /// Creates transparent proxy 
        /// </summary>
        public static TContract Create(IHttpClientProvider httpClientProvider, IObserver<CallDetails> callObserver = null)
        {
            if (httpClientProvider == null) throw new ArgumentNullException(nameof(httpClientProvider));

            object proxy = Create<TContract, ApiProxy<TContract>>();
            
            var proxyInner = (ApiProxy<TContract>)proxy;
            proxyInner.Initialize(httpClientProvider);
            if (callObserver != null)
                proxyInner.Subscribe(callObserver);

            return (TContract)proxy;
        }

        private void Initialize(IHttpClientProvider httpClientProvider)
        {
            var contractType = typeof(TContract);

            ApiRequestFactory = new ApiRequestFactory(contractType, httpClientProvider);
            _callTaskFactory = new GenericCallTaskFactory(ApiRequestFactory);
        }

        /// <inheritdoc />
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                return CoreInvoke(targetMethod, args);
            }
            catch (Exception e)
            {
                _callObservers.Error(e);
                throw;
            }
        }

        private object CoreInvoke(MethodInfo targetMethod, object[] args)
        {
            var retType = targetMethod.ReturnType;

            if(!typeof(Task).IsAssignableFrom(retType))
                throw new ApiClientException($"Wrong method return type '{retType.FullName}'");

            if (retType == typeof(Task))
            {
                return CallAndObserve(targetMethod, args, true);
            }
            if (retType == typeof(Task<CallDetails>))
            {
                return CallAndObserve(targetMethod, args, false);
            }
            if (retType.IsGenericType)
            {
                if(retType.GetGenericTypeDefinition() != typeof(Task<>))
                    throw new ApiClientException($"Wrong method return generic type '{retType.FullName}'");


                var gRetType = retType.GetGenericArguments().First();


                if (gRetType.IsGenericType && gRetType.GetGenericTypeDefinition() == typeof(CallDetails<>))
                {
                    var respContentType = gRetType.GetGenericArguments().First();

                    var m = CallWithDetailsAndObserveGenericMethod.MakeGenericMethod(respContentType);
                    return m.Invoke(this, new object[] { targetMethod, args });
                }
                else
                {
                    var m = CallAndObserveGenericMethod.MakeGenericMethod(gRetType);
                    return m.Invoke(this, new object[] { targetMethod, args });
                }
            }

            throw new InvalidOperationException("Unexpected return type");
        }

        async Task<CallDetails<T>> CallWithDetailsAndObserve<T>(MethodInfo targetMethod, object[] args)
        {
            var details = await ApiRequestFactory.Create<T>(targetMethod, args).GetDetailedAsync();

            _callObservers.Call(details);

            return details;
        }

        async Task<T> CallAndObserve<T>(MethodInfo targetMethod, object[] args)
        {
            var details = await ApiRequestFactory.Create<T>(targetMethod, args).GetDetailedAsync();

            _callObservers.Call(details);

            await details.ThrowIfUnexpectedStatusCode();

            return details.ResponseContent;
        }

        async Task<CallDetails> CallAndObserve(MethodInfo targetMethod, object[] args, bool throwIfUnexpected)
        {
            var details = await ApiRequestFactory.Create(targetMethod, args).GetDetailedAsync();

            _callObservers.Call(details);

            if (throwIfUnexpected)
                await details.ThrowIfUnexpectedStatusCode();

            return details;
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<CallDetails> observer)
        {
            return _callObservers.Subscribe(observer);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _callObservers.Compete();
        }

        

    }
}
