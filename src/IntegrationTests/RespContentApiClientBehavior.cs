using System;
using System.Collections.Generic;
using System.Linq;
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
    public class RespContentApiClientBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly ITestOutputHelper _output;
        private readonly ApiClient<ITestServer> _client;
        private readonly ITestServer _proxy;

        /// <summary>
        /// Initializes a new instance of <see cref="RespContentApiClientBehavior"/>
        /// </summary>
        public RespContentApiClientBehavior(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper output)
        {
            _output = output;
           var clientProvider = new DelegateHttpClientProvider(webApplicationFactory.CreateClient);
           _client = new ApiClient<ITestServer>(clientProvider);
           _proxy = ApiProxy<ITestServer>.Create(clientProvider);
        }

        [Fact]
        public async Task ShouldProvideXmlResponse()
        {
            //Act
            var resp = await _client
                .Method(s => s.GetXmlObj())
                .GetResultAsync();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponse()
        {
            //Act
            var resp = await _client
                .Method(s => s.GetJsonObj())
                .GetResultAsync();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideEnumerableResponse()
        {
            //Act
            var resp = await _client
                .Method(s => s.GetEnumerable())
                .GetResultAsync();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideArrayResponse()
        {
            //Act
            var resp = await _client
                .Method(s => s.GetArray())
                .GetResultAsync();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Theory]
        [MemberData(nameof(GetBinaryTestCases))]
        public async Task ShouldProvideBin(string title, Func<ApiClient<ITestServer>, Task<byte[]>> testProvider)
        {
            //Act
            var binResp = await testProvider(_client);

            var strResp = Encoding.UTF8.GetString(binResp);

            //Assert
            Assert.Equal("foo", strResp);
        }

        [Fact]
        public async Task ShouldProvideXmlResponseWithProxy()
        {
            //Act
            var resp = await _proxy.GetXmlObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideJsonResponseWithProxy()
        {
            //Act
            var resp = await _proxy.GetJsonObj();

            //Assert
            Assert.Equal("foo", resp.TestValue);
        }

        [Fact]
        public async Task ShouldProvideEnumerableResponseWithProxy()
        {
            //Arrange

            //Act
            var resp = await _proxy.GetEnumerable();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Fact]
        public async Task ShouldProvideArrayResponseWithProxy()
        {
            //Arrange

            //Act
            var resp = await _proxy.GetArray();
            var respArr = resp.ToArray();

            //Assert
            Assert.Equal(2, respArr.Length);
            Assert.Contains("foo", respArr);
            Assert.Contains("bar", respArr);
        }

        [Theory]
        [MemberData(nameof(GetBinaryTestCasesForProxy))]
        public async Task ShouldProvideBinWithProxy(string title, Func<ITestServer, Task<byte[]>> testProvider)
        {
            //Act
            var binResp = await testProvider(_proxy);

            var strResp = Encoding.UTF8.GetString(binResp);

            //Assert
            Assert.Equal("foo", strResp);
        }

        [Theory]
        [MemberData(nameof(GetDigitContentProvidingTestCases))]
        public async Task ShouldProvideDigitValue(string title, Func<ApiClient<ITestServer>, Task<object>> testProvider, object expected)
        {
            //Act
            var resp = await testProvider(_client);

            //Assert
            Assert.Equal(expected, resp);
        }

        [Theory]
        [MemberData(nameof(GetDigitContentProvidingTestCasesForProxy))]
        public async Task ShouldProvideDigitValueWithProxy(string title, Func<ITestServer, Task<object>> testProvider, object expected)
        {
            //Act
            var resp = await testProvider(_proxy);

            //Assert
            Assert.Equal(expected, resp);
        }

        public static IEnumerable<object[]> GetDigitContentProvidingTestCases()
        {
            yield return new object[]{ "short", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetShort()).GetResultAsync()), (short)10 };
            yield return new object[]{ "ushort", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetUShort()).GetResultAsync()), (ushort)10 };
            yield return new object[]{ "int", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetInt()).GetResultAsync()), 10 };
            yield return new object[]{ "uint", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetUInt()).GetResultAsync()), 10U };
            yield return new object[]{ "long", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetLong()).GetResultAsync()), 10L };
            yield return new object[]{ "ulong", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetULong()).GetResultAsync()), 10UL };
            yield return new object[]{ "double", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetDouble()).GetResultAsync()), 10.1D };
            yield return new object[]{ "float", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetFloat()).GetResultAsync()), 10.1F };
            yield return new object[]{ "decimal", (Func<ApiClient<ITestServer>, Task<object>>) (async c => await c.Method(s => s.GetDecimal()).GetResultAsync()), (decimal)10.1 };
        }

        public static IEnumerable<object[]> GetDigitContentProvidingTestCasesForProxy()
        {
            yield return new object[] { "short", (Func<ITestServer, Task<object>>)(async c => await c.GetShort()), (short)10 };
            yield return new object[] { "ushort", (Func<ITestServer, Task<object>>)(async c => await c.GetUShort()), (ushort)10 };
            yield return new object[] { "int", (Func<ITestServer, Task<object>>)(async c => await c.GetInt()), 10 };
            yield return new object[] { "uint", (Func<ITestServer, Task<object>>)(async c => await c.GetUInt()), 10U };
            yield return new object[] { "long", (Func<ITestServer, Task<object>>)(async c => await c.GetLong()), 10L };
            yield return new object[] { "ulong", (Func<ITestServer, Task<object>>)(async c => await c.GetULong()), 10UL };
            yield return new object[] { "double",(Func<ITestServer, Task<object>>)(async c => await c.GetDouble()), 10.1D };
            yield return new object[] { "float", (Func<ITestServer, Task<object>>)(async c => await c.GetFloat()), 10.1F };
            yield return new object[] { "decimal", (Func<ITestServer, Task<object>>)(async c => await c.GetDecimal()), (decimal)10.1 };
        }

        public static IEnumerable<object[]> GetBinaryTestCases()
        {
            yield return new object[] { "json", (Func<ApiClient<ITestServer>, Task<byte[]>>)(async c => await c.Method(s => s.GetBinJson()).GetResultAsync())};
            yield return new object[] { "octet", (Func<ApiClient<ITestServer>, Task<byte[]>>)(async c => await c.Method(s => s.GetBinOctetStream()).GetResultAsync())};
        }

        public static IEnumerable<object[]> GetBinaryTestCasesForProxy()
        {
            yield return new object[] { "json", (Func<ITestServer, Task<byte[]>>)(async c => await c.GetBinJson()) };
            yield return new object[] { "octet", (Func<ITestServer, Task<byte[]>>)(async c => await c.GetBinOctetStream())};
        }

        [Api("resp-content")]
        public interface ITestServer
        {

            [Get("data/xml")]
            Task<TestModel> GetXmlObj();

            [Get("data/json")]
            Task<TestModel> GetJsonObj();

            [Get("data/enumerable")]
            Task<IEnumerable<string>> GetEnumerable();

            [Get("data/enumerable")]
            Task<string[]> GetArray();

            [Get("data/int")]
            Task<short> GetShort();
            [Get("data/int")]
            Task<ushort> GetUShort();
            [Get("data/int")]
            Task<int> GetInt();
            [Get("data/int")]
            Task<uint> GetUInt();
            [Get("data/int")]
            Task<long> GetLong();
            [Get("data/int")]
            Task<ulong> GetULong();
            [Get("data/float")]
            Task<double> GetDouble();
            [Get("data/float")]
            Task<float> GetFloat();
            [Get("data/float")]
            Task<decimal> GetDecimal();

            [Get("data/bin-json")]
            Task<byte[]> GetBinJson();

            [Get("data/bin-octet-stream")]
            Task<byte[]> GetBinOctetStream();
        }
    }
}