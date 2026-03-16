using System;
using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring.UrlModifying;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.UrlModifying;

[TestSubject(typeof(UrlQueryInjector))]
public class UrlQueryInjectorBehavior
{
    [Theory]
    [InlineData("http://example.com", "param", "value", "http://example.com?param=value")]
    [InlineData("http://example.com?existing=1", "param", "value", "http://example.com?existing=1&param=value")]
    [InlineData("http://example.com", "param", null, "http://example.com?param=")]
    [InlineData("http://example.com", "param", "", "http://example.com?param=")]
    [InlineData("http://example.com", "param", "a b", "http://example.com?param=a%20b")]
    [InlineData("http://example.com", "param", "a+b", "http://example.com?param=a%2Bb")]
    [InlineData("http://example.com", "param", "a&b", "http://example.com?param=a%26b")]
    [InlineData("http://example.com/path", "param", "value", "http://example.com/path?param=value")]
    [InlineData("/relative-path", "param", "value", "/relative-path?param=value")]
    [InlineData("/relative-path?existing=1", "param", "value", "/relative-path?existing=1&param=value")]
    public void ShouldModifyUriWithQuery(string origin, string paramName, object value, string expected)
    {
        // Arrange
        var injector = new UrlQueryInjector();
        var uri = new Uri(origin, UriKind.RelativeOrAbsolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
    }

    [Theory]
    [InlineData("http://example.com", null, "value")]
    [InlineData("http://example.com", "", "value")]
    public void ShouldThrowExceptionWhenParamNameIsInvalid(string origin, string paramName, object value)
    {
        // Arrange
        var injector = new UrlQueryInjector();
        var uri = new Uri(origin, UriKind.RelativeOrAbsolute);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => injector.Modify(uri, paramName, value));
    }

    [Fact]
    public void ShouldHandleNullUri()
    {
        // Arrange
        var injector = new UrlQueryInjector();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => injector.Modify(null, "param", "value"));
    }

    static string UriToString(Uri uri)
    {
        return uri.OriginalString.Replace(":80", "").TrimEnd('/');
    }
}