using System;
using System.Reflection;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Dispatches proxy invocations
    /// </summary>
    public class ClientProxy<T> : DispatchProxy
        where T : class
    {
        private ApiClientDescription _clientDescription;
        private IClientProxyStrategy _strategy;

        void Initialize(ApiClientDescription clientDescription, IClientProxyStrategy strategy)
        {
            _clientDescription = clientDescription ?? throw new ArgumentNullException(nameof(clientDescription));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _strategy.Invoke(targetMethod, _clientDescription, args);
        }

        internal static T CreateProxy(ApiClientDescription clientDescription, IClientProxyStrategy strategy)
        {
            object proxy = Create<T, ClientProxy<T>>();
            ((ClientProxy<T>)proxy).Initialize(clientDescription, strategy);

            return (T) proxy;
        }
    }
}
