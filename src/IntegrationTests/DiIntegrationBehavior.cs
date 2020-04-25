using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class DiIntegrationBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly TestHttpClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public DiIntegrationBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;
            _clientProvider = new TestHttpClientProvider(webApplicationFactory);
        }

        [Fact]
        public async Task ShouldIntegrateProxy()
        {
            ////Arrange
            //var services = new ServiceCollection();

            //services.AddApiClients(
            //    registrar =>
            //    {
            //        registrar.RegisterContract<ITestServer>();
            //    },
            //    );

            //var serviceProvider = services.BuildServiceProvider();
            //var srv = ActivatorUtilities.CreateInstance<TestService>(serviceProvider);

            ////Act
            //var httpClient = srv.CreateHttpClient();

            ////Assert
            //Assert.NotNull(httpClient);
            //Assert.Equal("http://test.com", httpClient.BaseAddress?.OriginalString);

        }

        [Api("echo")]
        public interface ITestServer
        {
            [Get]
            Task<string> Echo([FromBody]string msg);
        }
    }
}
