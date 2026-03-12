using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MyLab.ApiClient.Options;
using System;
using System.Net.Http;

namespace MyLab.ApiClient.DI;

class HttpClientRegistrar<TContract>
{
    readonly ApiContractBinding _binding = new (typeof(TContract));

    public bool Optional { get; set; } = false;

    public void Register(IServiceCollection services)
    {
        foreach (var bindingKey in _binding.Keys)
        {
            services.AddHttpClient(bindingKey, RegisterHttpClient)
                .ConfigurePrimaryHttpMessageHandler(ConfigurePrimaryHttpMessageHandler);
        }
    }

    void RegisterHttpClient(IServiceProvider sp, HttpClient httpClient)
    {
        var opt = GetEpOpts(sp);

        if (opt != null)
        {
            if (opt!.Url == null)
                throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' URL is null");

            var normUrl = opt.Url.EndsWith("/")
                ? opt.Url
                : opt.Url + "/";

            httpClient.BaseAddress = new Uri(normUrl);
        }
    }

    HttpMessageHandler ConfigurePrimaryHttpMessageHandler(IServiceProvider sp)
    {
        var httpHandler = new HttpClientHandler();
            
        var opt = GetEpOpts(sp);
        if (opt is { SkipServerSslCertVerification: true })
        {
            httpHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        return httpHandler;
    }

    ApiEndpointOptions? GetEpOpts(IServiceProvider sp)
    {
        var opt = sp.GetService<IOptions<ApiClientOptions>>();

        if (opt?.Value == null)
            throw new InvalidOperationException("ApiClient options not found");

        if (!_binding.TryGetOptions(opt.Value, out var epOpt, out _))
        {
            return Optional
                ? null
                : throw new InvalidOperationException($"Api '{typeof(TContract).FullName}' options not found");
        }

        return epOpt;
    }

    
}