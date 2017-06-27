using System;
using System.Collections;
using System.Reflection;

namespace DotApiClient
{
    /// <summary>
    /// Creates web api client with specified base path
    /// </summary>
    public class WebApiClientFactory : IWebApiProxyFactory
    {
        private readonly string _basePath;

        /// <summary>
        /// Gets or sets interaction options
        /// </summary>
        public WebApiClientOptions Options { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiClientFactory"/>
        /// </summary>
        public WebApiClientFactory(string basePath)
        {
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

            return (T)new WebApiProxy<T>(desc, Options).GetTransparentProxy();
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

        /// <summary>
        /// Create web api proxy
        /// </summary>
        /// <typeparam name="T">web api contract</typeparam>
        public static T CreateProxy<T>(string apiPath)
            where T : class
        {
            var factory = new WebApiClientFactory(apiPath);
            return factory.CreateProxy<T>();
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
