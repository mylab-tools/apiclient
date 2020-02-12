using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Dispatches proxy invocations
    /// </summary>
    public class ClientProxy<T> : DispatchProxy
        where T : class
    {
        private static readonly MethodInfo ClientProxyGetInvocationGenericMethod =
            typeof(IClientProxyStrategy)
                .GetMethods()
                .Single(m => m.Name == nameof(IClientProxyStrategy.GetCall) && m.IsGenericMethod);

        private ApiClientDescription _clientDescription;
        private IClientProxyStrategy _strategy;

        void Initialize(ApiClientDescription clientDescription, IClientProxyStrategy strategy)
        {
            _clientDescription = clientDescription ?? throw new ArgumentNullException(nameof(clientDescription));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var mDesc = _clientDescription.GetMethod(targetMethod.MetadataToken);
            var cancellationToken = GetCancellationToken(mDesc, args);

            if (mDesc.ReturnType == typeof(void))
            {
                return _strategy
                    .GetCall(targetMethod, _clientDescription, args)
                    .Invoke(cancellationToken);
            }

            if (mDesc.ReturnType == typeof(WebApiCall))
            {
                return _strategy
                    .GetCall(targetMethod, _clientDescription, args);
            }

            if (targetMethod.ReturnType.IsGenericType &&
                targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(WebApiCall<>))
            {
                return CreateGenericInvocation(targetMethod, args, mDesc);
            }

            var genericCall = CreateGenericInvocation(targetMethod, args, mDesc);

            var invokeMethod = typeof(WebApiCall<>)
                .MakeGenericType(mDesc.ReturnType)
                .GetMethods()
                .First(m => m.Name == "Invoke" && m.GetParameters().Length == 1);
            
            return invokeMethod.Invoke(genericCall, new object[] {cancellationToken});
        }

        private object CreateGenericInvocation(MethodInfo targetMethod, object[] args, MethodDescription mDesc)
        {
            var genericParam = mDesc.ReturnType;
            var getInvocationMethod = ClientProxyGetInvocationGenericMethod.MakeGenericMethod(genericParam);
            return getInvocationMethod.Invoke(_strategy, new object[] {targetMethod, _clientDescription, args});
        }

        internal static T CreateProxy(ApiClientDescription clientDescription, IClientProxyStrategy strategy)
        {
            object proxy = Create<T, ClientProxy<T>>();
            ((ClientProxy<T>)proxy).Initialize(clientDescription, strategy);

            return (T) proxy;
        }

        static CancellationToken GetCancellationToken(MethodDescription methodDescription, object[] args)
        {
            var cancelParam = methodDescription.Params.FirstOrDefault(p => p.IsCancellationToken);

            return cancelParam != null
                ? (CancellationToken)args[cancelParam.Position]
                : CancellationToken.None;
        }
    }
}
