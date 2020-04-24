using System;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.ApiClient
{
    static class HttpClientRegistrar
    {
        public static void Register(IServiceCollection services, ApiClientsOptions options)
        {
            foreach (var desc in options.List)
            {
                services.AddHttpClient(desc.Key, client =>
                {
                    client.BaseAddress = new Uri(desc.Value.Url);   
                });
            }
        }
    }
}