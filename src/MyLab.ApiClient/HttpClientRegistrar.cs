using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.ApiClient
{
    static class HttpClientRegistrar
    {
        public static void Register(IServiceCollection services, ApiClientsOptions options)
        {
            foreach (var desc in options.List)
            {
                var normUrl = desc.Value.Url == null || desc.Value.Url.EndsWith("/")
                    ? desc.Value.Url
                    : desc.Value.Url + "/";

                services
                    .AddHttpClient(desc.Key, client =>
                    {
                        client.BaseAddress = new Uri(normUrl);   
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var httpHandler = new HttpClientHandler();

                        if (desc.Value.SkipServerSslCertVerification)
                        {
                            httpHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                            httpHandler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                        }

                        return httpHandler;
                    });
            }
        }
    }
}