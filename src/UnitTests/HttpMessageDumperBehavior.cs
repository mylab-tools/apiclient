using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class HttpMessageDumperBehavior
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="HttpMessageDumperBehavior"/>
        /// </summary>
        public HttpMessageDumperBehavior(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Theory]
        [InlineData("POST http://host.ru/orders")]
        [InlineData("param1=1&param2=2&param3=3")]
        [InlineData("Foo: FooVal")]
        public async Task ShouldSerializeRequest(string substring)
        {
            //Arrange
            var req = new HttpRequestMessage
            {
                RequestUri =new Uri("http://host.ru/orders"),
                Method = HttpMethod.Post,
                Headers =
                {
                    {"Foo", "FooVal" }
                },
                Content = new StringContent("param1=1&param2=2&param3=3")
            };
            var dumper = new HttpMessageDumper();

            //Act
            var dump = await dumper.Dump(req);
            _output.WriteLine(dump);

            //Assert
            Assert.Contains(substring, dump);
        }

        [Theory]
        [InlineData("param1=1&param2=2&param3=3")]
        [InlineData("Foo: FooVal")]
        public async Task ShouldSerializeResponse(string substring)
        {
            //Arrange
            var req = new HttpResponseMessage
            {
                Headers =
                {
                    {"Foo", "FooVal" }
                },
                Content = new StringContent("param1=1&param2=2&param3=3")
            };
            var dumper = new HttpMessageDumper();

            //Act
            var dump = await dumper.Dump(req);
            _output.WriteLine(dump);

            //Assert
            Assert.Contains(substring, dump);
        }

        [Fact]
        public async Task ShouldTrimRequestBodyIfTooLarge()
        {
            //Arrange
            var req = new HttpRequestMessage
            {
                RequestUri = new Uri("http://host.ru/orders"),
                Method = HttpMethod.Post,
                Content = new StringContent("param1=1&param2=2&param3=3")
            };
            var dumper = new HttpMessageDumper
            {
                MaxRequestBodySize = 5
            };

            //Act
            var dump = await dumper.Dump(req);
            _output.WriteLine(dump);

            //Assert
            Assert.Contains(HttpMessageDumper.ContentIsTooLargeText, dump);

        }

        [Fact]
        public async Task ShouldTrimResponseBodyIfTooLarge()
        {
            //Arrange
            var req = new HttpResponseMessage
            {
                Content = new StringContent("param1=1&param2=2&param3=3")
            };
            var dumper = new HttpMessageDumper
            {
                MaxResponseBodySize = 5
            };

            //Act
            var dump = await dumper.Dump(req);
            _output.WriteLine(dump);

            //Assert
            Assert.Contains(HttpMessageDumper.ContentIsTooLargeText, dump);

        }
    }
}
