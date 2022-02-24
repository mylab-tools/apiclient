using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
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
        private readonly IHttpClientProvider _clientProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="SendParamsApiClientBehavior"/>
        /// </summary>
        public SendParamsApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;

            _clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
        }

        [Theory]
        [MemberData(nameof(GetSendParametersTests))]
        public async Task ShouldSendParameters(Expression<Func<ITestServer, Task<string>>> serviceCallExpr)
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);

            //Act
            var resp = await client
                .Request(serviceCallExpr)
                .GetDetailedAsync();

            _output.WriteLine("====== Request ======");
            _output.WriteLine(resp.RequestDump);
            _output.WriteLine("====== Response ======");
            _output.WriteLine(resp.ResponseDump);

            //Assert
            Assert.False(resp.IsUnexpectedStatusCode);
            Assert.Equal("foo", resp.ResponseContent);
        }

        [Fact]
        public async Task ShouldSendContentLengthWhenSendJson()
        {
            //Arrange
            var client = new ApiClient<ITestServer>(_clientProvider);
            var model = new TestModel();

            //Act
            var resp = await client
                .Request(s => s.EchoJsonHeaders(model))
                .GetDetailedAsync();

            _output.WriteLine("====== Request ======");
            _output.WriteLine(resp.RequestDump);
            _output.WriteLine("====== Response ======");
            _output.WriteLine(resp.ResponseDump);

            //Assert
            Assert.False(resp.IsUnexpectedStatusCode);
            Assert.Contains(resp.ResponseContent, pair => pair.Key == "Content-Length");
        }

        public static IEnumerable<object[]> GetSendParametersTests()
        {
            var testModel = new TestModel {TestValue = "foo"};

            Expression<Func<ITestServer, Task<string>>> expr1 = s => s.EchoQuery("foo");
            Expression<Func<ITestServer, Task<string>>> expr2 = s => s.EchoPath("foo");
            Expression<Func<ITestServer, Task<string>>> expr3 = s => s.EchoHeader("foo");
            Expression<Func<ITestServer, Task<string>>> expr4 = s => s.EchoXmlObj(testModel);
            Expression<Func<ITestServer, Task<string>>> expr5 = s => s.EchoJsonObj(testModel);
            Expression<Func<ITestServer, Task<string>>> expr6 = s => s.EchoForm(testModel);
            Expression<Func<ITestServer, Task<string>>> expr7 = s => s.EchoText("foo");
            Expression<Func<ITestServer, Task<string>>> expr8 = s => s.EchoBin(Encoding.UTF8.GetBytes("foo"));
            Expression<Func<ITestServer, Task<string>>> expr9 = s => s.EchoHeaderCollection(new Dictionary<string, object>{ {"Message", "f"}, { "Message2", "oo" } });
            Expression<Func<ITestServer, Task<string>>> expr10 = s => s.EchoMultipart(new TestMultipartParameter{ Part1 = "fo", Part2 = "o"});
            Expression<Func<ITestServer, Task<string>>> expr11 = s => s.EchoFormWithName(new TestFormWithNAme{ TestValue = "foo"});

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
                new object[] {expr9},
                new object[] {expr10},
                new object[] {expr11},
            };
        }

        [Api("param-sending")]
        public interface ITestServer
        {
            [Post("echo/multipart")]
            Task<string> EchoMultipart([MultipartContent] TestMultipartParameter msg);
            [Post("echo/query")]
            Task<string> EchoQuery([Query]string msg);

            [Post("echo/{msg}/path")]
            Task<string> EchoPath([Path] string msg);

            [Post("echo/header")]
            Task<string> EchoHeader([Header("Message")] string msg);

            [Post("echo/header")]
            Task<string> EchoHeaderCollection([HeaderCollection] Dictionary<string,object> headers);

            [Post("echo/body/obj/xml")]
            Task<string> EchoXmlObj([XmlContent] TestModel model);

            [Post("echo/body/obj/json")]
            Task<string> EchoJsonObj([JsonContent] TestModel model);

            [Post("echo/body/form")]
            Task<string> EchoForm([FormContent] TestModel model);

            [Post("echo/body/form-with-name")]
            Task<string> EchoFormWithName([FormContent] TestFormWithNAme model);

            [Post("echo/body/text")]
            Task<string> EchoText([StringContent] string msg);

            [Post("echo/body/bin")]
            Task<string> EchoBin([BinContent] byte[] msg);

            [Post("echo/json-headers")]
            Task<Dictionary<string, string>> EchoJsonHeaders([JsonContent] TestModel model);
        }

        public class TestMultipartParameter : IMultipartContentParameter
        {
            public string Part1 { get; set; }
            public string Part2 { get; set; }

            public void AddParts(MultipartFormDataContent content)
            {
                content.Add(new StringContent(Part1), "part1");
                content.Add(new StringContent(Part2), "part2");
            }
        }

        public class TestFormWithNAme
        {
            [UrlFormItem(Name = "test_value")]
            public string TestValue { get; set; }
        }
    }
}