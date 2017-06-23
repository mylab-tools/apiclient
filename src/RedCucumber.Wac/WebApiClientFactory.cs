using System;
using System.Reflection;

namespace RedCucumber.Wac
{
    /// <summary>
    /// Creates web api client with specified contract
    /// </summary>
    /// <typeparam name="T">Web api contract</typeparam>
    public class WebApiClientFactory<T>
        where T : class
    {
        private readonly IWebApiRequestProcessor _requestProcessor;

        private readonly string _basePath;

        public WebApiClientFactory(string basePath, IWebApiRequestProcessor requestProcessor = null)
        {
            _requestProcessor = requestProcessor ?? new WebApiRequestProcessor();

            _basePath = basePath;
        }

        public T Create()
        {
            var serviceAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<WebApiServiceAttribute>();
            var resourceAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<WebApiResourceAttribute>();

            if (serviceAttribute == null && resourceAttribute == null)
            {
                throw new WebApiContractException($"Web api contract '{typeof(T).FullName}' should be marked with '{typeof(WebApiServiceAttribute).FullName}' or with '{typeof(WebApiResourceAttribute).FullName}'.");
            }

            var desc = WebApiDescription.Create(typeof(T));

            ProcessBaseAddress(desc);

            return (T)new WebApiProxy<T>(desc, _requestProcessor).GetTransparentProxy();
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
}
