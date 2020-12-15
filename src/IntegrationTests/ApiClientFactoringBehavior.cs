using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;
using ResponseCodeException = MyLab.ApiClient.ResponseCodeException;

namespace IntegrationTests
{
    public class ApiClientFactoringBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory;
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public ApiClientFactoringBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _webApplicationFactory = webApplicationFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldNotFailIfContractHasNoPaths()
        {
            //Arrange
            var clientProvider = new DelegateHttpClientProvider(_webApplicationFactory.CreateClient);
            var client = new ApiClient<ITestContractWithoutPath>(clientProvider);
            HttpStatusCode? actualCode = null;

            //Act
            try
            {
                await client.Method(s => s.Get()).CallAsync();
            }
            catch (ResponseCodeException e)
            {
                actualCode = e.StatusCode;
            }

            //Assert
            Assert.True(actualCode.HasValue);
            Assert.Equal(HttpStatusCode.NotFound, actualCode.Value);
        }

        [Api]
        interface ITestContractWithoutPath
        {
            [Get]
            Task Get();
        }
    }
}
