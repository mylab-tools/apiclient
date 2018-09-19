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
                .Single(m => m.Name == nameof(IClientProxyStrategy.GetInvocation) && m.IsGenericMethod);

        private static readonly MethodInfo GenericWebApiInvocationInvokeMethod =
            typeof(WebApiInvocation<>).GetMethod("Invoke");

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
                    .GetInvocation(targetMethod, _clientDescription, args)
                    .Invoke(cancellationToken);
            }

            if (mDesc.ReturnType == typeof(WebApiInvocation))
            {
                return _strategy
                    .GetInvocation(targetMethod, _clientDescription, args);
            }

            if (mDesc.ReturnType.IsGenericType &&
                mDesc.ReturnType.GetGenericTypeDefinition() == typeof(WebApiInvocation<>))
            {
                return CreateGenericInvocation(targetMethod, args, mDesc);
            }

            var genericInvocation = CreateGenericInvocation(targetMethod, args, mDesc);
            return GenericWebApiInvocationInvokeMethod.Invoke(genericInvocation,
                new object[] {cancellationToken});
        }

        private object CreateGenericInvocation(MethodInfo targetMethod, object[] args, MethodDescription mDesc)
        {
            var genericParam = mDesc.ReturnType.GenericTypeArguments[0];
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
