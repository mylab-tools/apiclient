using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using TestServer;
using TestServer.Models;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class SendParamsApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ApiClient<ITestServer> _client;

        /// <summary>
        /// Initializes a new instance of <see cref="SendParamsApiClientBehavior"/>
        /// </summary>
        public SendParamsApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;

            var clientProvider = new TestHttpClientProvider(webApplicationFactory);
            _client = ApiClient<ITestServer>.Create(clientProvider);
        }

        [Theory]
        [MemberData(nameof(GetSendParametersTests))]
        public async Task ShouldSendParameters(Expression<Func<ITestServer, string>> serviceCallExpr)
        {
            //Arrange


            //Act
            var resp = await _client
                .Call(serviceCallExpr)
                .GetDetailed();

            _output.WriteLine("====== Request ======");
            _output.WriteLine(resp.RequestDump);
            _output.WriteLine("====== Response ======");
            _output.WriteLine(resp.ResponseDump);

            //Assert
            Assert.False(resp.IsUnexpectedStatusCode);
            Assert.Equal("foo", resp.ResponseContent);
        }

        public static IEnumerable<object[]> GetSendParametersTests()
        {
            var testModel = new TestModel {TestValue = "foo"};

            Expression<Func<ITestServer, string>> expr1 = s => s.EchoQuery("foo");
            Expression<Func<ITestServer, string>> expr2 = s => s.EchoPath("foo");
            Expression<Func<ITestServer, string>> expr3 = s => s.EchoHeader("foo");
            Expression<Func<ITestServer, string>> expr4 = s => s.EchoXmlObj(testModel);
            Expression<Func<ITestServer, string>> expr5 = s => s.EchoJsonObj(testModel);
            Expression<Func<ITestServer, string>> expr6 = s => s.EchoForm(testModel);
            Expression<Func<ITestServer, string>> expr7 = s => s.EchoText("foo");
            Expression<Func<ITestServer, string>> expr8 = s => s.EchoBin(Encoding.UTF8.GetBytes("foo"));

            return new List<object[]>
            {
                new object[] {expr1},
                new object[] {expr2},
                new object[] {expr3},
                new object[] {expr4},
                new object[] {expr5},
                new object[] {expr6},
                new object[] {expr7},
                new object[] {expr8},
            };
        }

        [Api("param-sending")]
        public interface ITestServer
        {
            [Post("echo/query")]
            string EchoQuery([Query]string msg);

            [Post("echo/{msg}/path")]
            string EchoPath([Path] string msg);

            [Post("echo/header")]
            string EchoHeader([Header("Message")] string msg);

            [Post("echo/body/obj/xml")]
            string EchoXmlObj([XmlContent] TestModel model);

            [Post("echo/body/obj/json")]
            string EchoJsonObj([JsonContent] TestModel model);

            [Post("echo/body/form")]
            string EchoForm([FormContent] TestModel model);

            [Post("echo/body/text")]
            string EchoText([StringContent] string msg);

            [Post("echo/body/bin")]
            string EchoBin([BinContent] byte[] msg);
        }
    }
}