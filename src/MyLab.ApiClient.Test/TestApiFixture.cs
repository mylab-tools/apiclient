using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Usage.Invocation;
using MyLab.ApiClient.DI;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.Usage;
using Xunit;

namespace MyLab.ApiClient.Test;

/// <summary>
/// Provides tools to create test API application
/// </summary>
public class TestApiFixture<TStartup, TContract> : IDisposable
    where TStartup : class
    where TContract : class
{
    readonly WebApplicationFactory<TStartup> _appFactory;

    /// <summary>
    /// Overrides services
    /// </summary>
    public Action<IServiceCollection>? ServiceOverrider { get; set; }

    /// <summary>
    /// Additional tunes <see cref="HttpClient"/>
    /// </summary>
    public Action<HttpClient>? HttpClientTuner { get; set; }

    /// <summary>
    /// Test output for log writing
    /// </summary>
    public ITestOutputHelper? Output { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="TestApiFixture{TStartup, TApiContract}"/>
    /// </summary>
    public TestApiFixture()
    {
        _appFactory = new WebApplicationFactory<TStartup>();
    }

    /// <summary>
    /// Starts test API instance and returns <see cref="InvokerTestApiAsset{TContract}"/>
    /// </summary>
    public InvokerTestApiAsset<TContract> StartAppWithInvoker(
        Action<IServiceCollection>? serviceOverrider = null,
        Action<HttpClient>? httpClientTuner = null)
    {
        var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
        {
            ServiceOverrider?.Invoke(srv);
            serviceOverrider?.Invoke(srv);
        }));

        var opt = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        var innerHttpClient = factory.CreateClient(opt);

        HttpClientTuner?.Invoke(innerHttpClient);
        httpClientTuner?.Invoke(innerHttpClient);

        var httpClientProvider = new SingleHttpClientProvider(innerHttpClient);
        var requestProcessor = new HttpClientRequestProcessor(httpClientProvider);
        var testRequestProcessor = new TestHttpClientRequestProcessor(requestProcessor, typeof(TContract), Output);
        var opts = new ApiClientOptions();

        var invoker = ApiClientInvoker<TContract>.FromContract(testRequestProcessor, opts);

        return new InvokerTestApiAsset<TContract>()
        {
            Invoker = invoker,
            HttpClient = innerHttpClient,
            ServiceProvider = factory.Services
        };
    }

    /// <summary>
    /// Starts test API instance and returns ApiClient proxy
    /// </summary>
    public ProxyTestApiAsset<TContract> StartAppWithProxy(
        Action<IServiceCollection>? serviceOverrider = null,
        Action<HttpClient>? httpClientTuner = null)
    {
        var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
        {
            ServiceOverrider?.Invoke(srv);
            serviceOverrider?.Invoke(srv);
        }));

        var opt = new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        };

        var innerHttpClient = factory.CreateClient(opt);

        HttpClientTuner?.Invoke(innerHttpClient);
        httpClientTuner?.Invoke(innerHttpClient);

        var httpClientProvider = new SingleHttpClientProvider(innerHttpClient);
        var requestProcessor = new HttpClientRequestProcessor(httpClientProvider);
        var testRequestProcessor = new TestHttpClientRequestProcessor(requestProcessor, typeof(TContract), Output);
        var opts = new ApiClientOptions();

        var proxy = ApiClientProxy.CreateForContract<TContract>(testRequestProcessor, opts);

        return new ProxyTestApiAsset<TContract>()
        {
            Proxy = proxy,
            HttpClient = innerHttpClient,
            ServiceProvider = factory.Services
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _appFactory?.Dispose();
    }
}