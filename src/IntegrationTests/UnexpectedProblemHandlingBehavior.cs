using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace IntegrationTests
{
    public class UnexpectedProblemHandlingBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ApiClient<ITestServer> _client;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public UnexpectedProblemHandlingBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;
           var clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
           _client = new ApiClient<ITestServer>(clientProvider);
        }

        [Fact]
        public async Task ShouldIgnoreProblemWhenValueTypeReturns()
        {
            //Act
            var resp = await _client.Call(s => s.GetAbsent()).GetDetailed();
            Log(resp);

            //Assert
        }

        [Fact]
        public async Task ShouldIgnoreProblemWhenObjectTypeReturns()
        {
            //Act
            var resp = await _client.Call(s => s.GetAbsentModel()).GetDetailed();
            Log(resp);

            //Assert
        }

        void Log<T>(CallDetails<T> details)
        {
            _output.WriteLine("=== REQUEST ===");
            _output.WriteLine(details.RequestDump);
            _output.WriteLine("=== RESPONSE ===");
            _output.WriteLine(details.ResponseDump);
            _output.WriteLine("=== END ===");
        }


        [Api("resp-info")]
        public interface ITestServer
        {

            [Get("404")]
            Task<int> GetAbsent();

            [Get("404")]
            Task<TestModel> GetAbsentModel();
        }
    }
}