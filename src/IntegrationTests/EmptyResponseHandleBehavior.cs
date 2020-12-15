using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EmptyResponseHandleBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly IHttpClientProvider _clientProvider;
        private readonly ITestOutputHelper _output;

        public EmptyResponseHandleBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
            _output = output;
        }

        [Fact]
        public async Task ShouldReturnNullIfStringResponseIsEmpty()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act 
            var resDet = await client.Method(s => s.GetNullString()).GetDetailedAsync();
            Log(resDet);

            //Assert
            if (resDet.ResponseContent == string.Empty)
                _output.WriteLine("String is empty");

            Assert.Null(resDet.ResponseContent);
        }

        [Fact]
        public async Task ShouldReturnNullIfValueResponseIsEmpty()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act 
            var resDet = await client.Method(s => s.GetNullValue()).GetDetailedAsync();
            Log(resDet);

            //Assert
            Assert.Equal(default, resDet.ResponseContent);
        }

        [Fact]
        public async Task ShouldReturnNullIfArrayResponseIsEmpty()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act 
            var resDet = await client.Method(s => s.GetNullArray()).GetDetailedAsync();
            Log(resDet);

            //Assert
            if(resDet.ResponseContent != null)
                _output.WriteLine("Array length: " + resDet.ResponseContent.Length);

            Assert.Null(resDet.ResponseContent);
        }

        [Fact]
        public async Task ShouldSupportNoResult()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act & Assert
            var resDet = await client.Method(s => s.GetNoResult()).GetDetailedAsync();
            Log(resDet);
            
        }

        private void Log(CallDetails resDet)
        {
            _output.WriteLine("Request:");
            _output.WriteLine(resDet.RequestDump);
            _output.WriteLine("Response:");
            _output.WriteLine(resDet.ResponseDump);
            
        }

        [Api("echo/empty")]
        interface ITestServer
        {
            [Get]
            Task GetNoResult();

            [Get]
            Task<string> GetNullString();

            [Get]
            Task<int> GetNullValue();

            [Get]
            Task<byte[]> GetNullArray();
        }
    }
}
