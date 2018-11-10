using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Testing
{
    /// <summary>
    /// Testing extensions for <see cref="ApiClient{TContract}"/>
    /// </summary>
    public static class ApiClientBuilderExtensions
    {
        /// <summary>
        /// Creates test client
        /// </summary>
        /// <typeparam name="TContract">Server contract</typeparam>
        /// <typeparam name="TEntryPoint">Web app entry point. Startup class by default</typeparam>
        public static TContract Create<TContract, TEntryPoint>(
            this ApiClient<TContract> apiClientFactory,
            WebApplicationFactory<TEntryPoint> webAppFactory,
            ITestOutputHelper output = null,
            IDictionary<string, string> addHeaders = null)
            where TContract : class
            where TEntryPoint : class
        {
            return apiClientFactory.Create(
                new TestHttpClientProvider<TEntryPoint>(webAppFactory)
                {
                    Headers = addHeaders != null 
                        ? new Dictionary<string, string>(addHeaders)
                        : null
                },
                output != null
                    ? new TestConsoleWriterHttpMessageListener(output)
                    : null
            );
        }
    }
}