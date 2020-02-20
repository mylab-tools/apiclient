using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class ApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ApiClient<ITestServer> _client;

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClientBehavior"/>
        /// </summary>
        public ApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;

            var clientProvider = new TestHttpClientProvider(webApplicationFactory);
            _client = ApiClient<ITestServer>.Create(clientProvider);
        }

        [Fact]
        public async Task ShouldThrowWhenUnexpectedStatusCode()
        {
            //Arrange

            //Act & Assert
            await Assert.ThrowsAsync<ResponseCodeException>(() =>
            {
                return _client.Request(s => s.GetUnexpected404()).Send();
            });
        }

        [Fact]
        public async Task ShouldPassWhenExpectedStatusCode()
        {
            //Arrange

            //Act & Assert
            await _client.Request(s => s.GetExpected404()).Send();
        }

        [Fact]
        public async Task ShouldProvideStatusMessage()
        {
            //Arrange

            //Act
            var resp = await _client
                .Request(s => s.GetExpected404())
                .Send();

            //Assert
            Assert.Equal("This is a message", resp);
        }

        [Fact]
        public async Task ShouldProvideXmlResponse()
        {
            //Arrange
            

            //Act
            var resp = await _client
                .Request(s => s.GetXmlObj())
                .Send();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponse()
        {
            //Arrange


            //Act
            var resp = await _client
                .Request(s => s.GetJsonObj())
                .Send();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Theory]
        [MemberData(nameof(GetSendParametersTests))]
        public async Task ShouldSendParameters(Expression<Func<ITestServer, string>> serviceCallExpr)
        {
            //Arrange


            //Act
            var resp = await _client
                .Request(serviceCallExpr)
                .Send();

            //Assert
            Assert.Equal("foo", resp);
        }

        public static IEnumerable<object[]> GetSendParametersTests()
        {
            var testModel = new TestModel {TestValue = "foo"};

            Expression<Func<ITestServer, string>> expr1 = s => s.PingQuery("foo");
            Expression<Func<ITestServer, string>> expr2 = s => s.PingPath("foo");
            Expression<Func<ITestServer, string>> expr3 = s => s.PingHeader("foo");
            Expression<Func<ITestServer, string>> expr4 = s => s.PingObj(testModel);
            Expression<Func<ITestServer, string>> expr5 = s => s.PingForm(testModel);

            return new List<object[]>
            {
                new object[] {expr1},
                new object[] {expr2},
                new object[] {expr3},
                new object[] {expr4},
                new object[] {expr5}
            };
        }
    }

    [Api("test")]
    public interface ITestServer
    {
        [Get("400")]
        void GetUnexpected404();

        [ExpectedCode(HttpStatusCode.BadRequest)]
        [Get("400")]
        void GetExpected404();

        [Get("data/xml")]
        TestModel GetXmlObj();

        [Get("data/json")]
        TestModel GetJsonObj();

        [Post("ping/query")]
        string PingQuery([Query]string msg);

        [Post("ping/{msg}/path")]
        string PingPath([Path] string msg);

        [Post("ping/header")]
        string PingHeader([Header("Message")] string msg);

        [Post("ping/body/obj")]
        string PingObj([JsonContent] TestModel model);

        [Post("ping/body/form")]
        string PingForm([FormContent] TestModel model);
    }
}
