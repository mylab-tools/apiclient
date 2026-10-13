using System;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.Tools;
using Xunit;

namespace UnitTests.Tools;

public class HttpMessageDumperBehavior
{
    readonly ITestOutputHelper _output;

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
            RequestUri = new Uri("http://host.ru/orders"),
            Method = HttpMethod.Post,
            Headers =
                {
                    {"Foo", "FooVal" }
                },
            Content = new StringContent("param1=1&param2=2&param3=3")
        };
        var dumper = new HttpMessageDumper();

        //Act
        var dump = await dumper.DumpAsync(req);
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
        var dump = await dumper.DumpAsync(req);
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
        var dump = await dumper.DumpAsync(req);
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
        var dump = await dumper.DumpAsync(req);
        _output.WriteLine(dump);

        //Assert
        Assert.Contains(HttpMessageDumper.ContentIsTooLargeText, dump);

    }

    [Theory]
    [InlineData("Authorization", "Bearer secret-token")]
    [InlineData("Cookie", "session=abc")]
    [InlineData("X-Api-Key", "key-123")]
    public async Task ShouldMaskSensitiveRequestHeadersByDefault(string headerName, string headerValue)
    {
        //Arrange
        var req = new HttpRequestMessage
        {
            RequestUri = new Uri("http://host.ru/orders"),
            Method = HttpMethod.Get,
            Headers =
            {
                { headerName, headerValue }
            }
        };
        var dumper = new HttpMessageDumper();

        //Act
        var dump = await dumper.DumpAsync(req);
        _output.WriteLine(dump);

        //Assert
        Assert.Contains($"{headerName}: {HttpMessageDumper.SensitiveInfoPlaceholder}", dump);
        Assert.DoesNotContain(headerValue, dump);
    }

    [Fact]
    public async Task ShouldMaskSensitiveResponseHeadersByDefault()
    {
        //Arrange
        var resp = new HttpResponseMessage
        {
            Headers =
            {
                { "Set-Cookie", "session=abc; Path=/" }
            },
            Content = new StringContent("ok")
        };
        var dumper = new HttpMessageDumper();

        //Act
        var dump = await dumper.DumpAsync(resp);
        _output.WriteLine(dump);

        //Assert
        Assert.Contains($"Set-Cookie: {HttpMessageDumper.SensitiveInfoPlaceholder}", dump);
        Assert.DoesNotContain("session=abc", dump);
    }

    [Fact]
    public async Task ShouldShowSensitiveInfoWhenEnabled()
    {
        //Arrange
        var req = new HttpRequestMessage
        {
            RequestUri = new Uri("http://host.ru/orders"),
            Method = HttpMethod.Get,
            Headers =
            {
                { "Authorization", "Bearer secret-token" }
            }
        };
        var dumper = new HttpMessageDumper
        {
            ShowSensitiveInfo = true
        };

        //Act
        var dump = await dumper.DumpAsync(req);
        _output.WriteLine(dump);

        //Assert
        Assert.Contains("Authorization: Bearer secret-token", dump);
        Assert.DoesNotContain(HttpMessageDumper.SensitiveInfoPlaceholder, dump);
    }

    [Fact]
    public async Task ShouldNotMaskNonSensitiveHeaders()
    {
        //Arrange
        var req = new HttpRequestMessage
        {
            RequestUri = new Uri("http://host.ru/orders"),
            Method = HttpMethod.Get,
            Headers =
            {
                { "X-Request-Id", "req-1" }
            }
        };
        var dumper = new HttpMessageDumper();

        //Act
        var dump = await dumper.DumpAsync(req);
        _output.WriteLine(dump);

        //Assert
        Assert.Contains("req-1", dump);
        Assert.DoesNotContain(HttpMessageDumper.SensitiveInfoPlaceholder, dump);
    }
}