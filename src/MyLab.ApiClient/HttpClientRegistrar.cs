using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MyLab.ApiClient
{
    static class HttpClientRegistrar
    {
        public static void Register(IServiceCollection services, IEnumerable<string> registeredApiKeys)
        {
            foreach (var apiKey in registeredApiKeys)
            {
                services
                    .AddHttpClient(apiKey, (sp, client) =>
                    {
                        var opt = GetApiOpts(sp, apiKey);

                        if (opt.Url == null)
                            throw new InvalidOperationException($"Api '{apiKey}' URL is null");

                        var normUrl = opt.Url == null || opt.Url.EndsWith("/")
                            ? opt.Url
                            : opt.Url + "/";

                        client.BaseAddress = new Uri(normUrl);
                    })
                    .ConfigurePrimaryHttpMessageHandler(sp =>
                    {
                        var httpHandler = new HttpClientHandler();

                        var opt = GetApiOpts(sp, apiKey);

                        if (opt.SkipServerSslCertVerification)
                        {
                            httpHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                            httpHandler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;
                        }

                        return httpHandler;
                    });
            }
        }

        static ApiConnectionOptions GetApiOpts(IServiceProvider sp, string apiKey)
        {
            var opt = sp.GetService<IOptions<ApiClientsOptions>>();

            if (opt?.Value == null)
                throw new InvalidOperationException($"ApiClient options not found");
            
            if (!opt.Value.List.TryGetValue(apiKey, out var apiOpt))
                throw new InvalidOperationException($"Api '{apiKey}' options not found");

            return apiOpt;
        }
    }
}