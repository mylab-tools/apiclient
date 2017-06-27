using System;
using System.Reflection;

namespace DotAspectClient
{
    /// <summary>
    /// Creates web api client with specified base path
    /// </summary>
    public class WebApiClientFactory : IWebApiProxyFactory
    {
        private readonly IWebApiRequestProcessor _requestProcessor;

        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiClientFactory"/>
        /// </summary>
        public WebApiClientFactory(string basePath, IWebApiRequestProcessor requestProcessor = null)
        {
            _requestProcessor = requestProcessor ?? new DefaultWebApiRequestProcessor();

            _basePath = basePath;
        }

        /// <summary>
        /// Create web api proxy
        /// </summary>
        /// <typeparam name="T">web api contract</typeparam>
        public T CreateProxy<T>()
            where T : class
        {
            var serviceAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<WebApiAttribute>();
            var resourceAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<RestApiAttribute>();

            if (serviceAttribute == null && resourceAttribute == null)
            {
                throw new WebApiContractException(
                    $"Web api contract '{typeof(T).FullName}' should be marked with '{typeof(WebApiAttribute).FullName}' or with '{typeof(RestApiAttribute).FullName}'.");
            }

            var desc = WebApiDescription.Create(typeof(T));

            ProcessBaseAddress(desc);

            return (T)new WebApiProxy<T>(desc, _requestProcessor).GetTransparentProxy();
        }

        /// <summary>
        /// Creates web api tontract specific factory
        /// </summary>
        /// <typeparam name="T">web api contract</typeparam>
        public WebApiClientFactory<T> CreateTypedFactory<T>()
            where T : class
        {
            return new WebApiClientFactory<T>(this);
        }

        private void ProcessBaseAddress(WebApiDescription desc)
        {
            if (!string.IsNullOrWhiteSpace(_basePath))
            {
                desc.BaseUrl = !string.IsNullOrWhiteSpace(desc.BaseUrl) 
                    ? new Uri(new Uri(_basePath), desc.BaseUrl).ToString()
                    : _basePath;
            }
            if (string.IsNullOrWhiteSpace(desc.BaseUrl))
                throw new WebApiContractException("The base address is empty");
        } 
    }

    /// <summary>
    /// Creates web api client with specified contract
    /// </summary>
    /// <typeparam name="T">aeb api contract</typeparam>
    public class WebApiClientFactory<T>
        where T : class 
    {
        private readonly IWebApiProxyFactory _factory;

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiClientFactory"/>
        /// </summary>
        public WebApiClientFactory(IWebApiProxyFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Creates client
        /// </summary>
        public T CreateProxy()
        {
            return _factory.CreateProxy<T>();
        }
    }
}
