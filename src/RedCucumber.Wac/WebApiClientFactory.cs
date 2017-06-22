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
        public string BasePath { get; }

        public WebApiClientFactory(string basePath)
        {
            BasePath = basePath;
        }

        public T Create()
        {
            var serviceAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<WebApiServiceAttribute>();
            if (serviceAttribute != null)
            {
                var factory = new ServiceApiClientFactory<T>(serviceAttribute);
                return factory.Create();
            }

            var resourceAttribute = typeof(T).GetTypeInfo().GetCustomAttribute<WebApiResourceAttribute>();
            if (resourceAttribute != null)
            {
                var factory = new ResourceApiClientFactory<T>(resourceAttribute);
                return factory.Create();
            }

            throw new WebApiContractException($"Web api contract '{typeof(T).FullName}' should be marked with '{typeof(WebApiServiceAttribute).FullName}' or with '{typeof(WebApiResourceAttribute).FullName}'.");
        }
    }
}
