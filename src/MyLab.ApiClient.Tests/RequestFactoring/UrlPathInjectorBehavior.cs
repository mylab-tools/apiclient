using System;   
using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring;

[TestSubject(typeof(UrlPathInjector))]
public class UrlPathInjectorBehavior
{
    [Theory]
    [InlineData("http://example.com/{param}", "param", "value", "http://example.com/value")]
    [InlineData("http://example.com/{param}/test", "param", "value", "http://example.com/value/test")]
    [InlineData("http://example.com/{param}/test", "param", null, "http://example.com/test")]
    [InlineData("http://example.com/{param}/test", "param", "", "http://example.com/test")]
    [InlineData("http://example.com/{param}/test", "param", " ", "http://example.com/test")]
    [InlineData("http://example.com/{param}/test", "param", "value with spaces",
        "http://example.com/value%20with%20spaces/test")]
    [InlineData("http://example.com/{param}/test", "param", "value/with/slash",
        "http://example.com/value%2Fwith%2Fslash/test")]
    [InlineData("/relative/{param}/path", "param", "value", "/relative/value/path")]
    [InlineData("/relative/{param}/path", "param", null, "/relative/path")]
    [InlineData("/relative/{param}/path", "param", "", "/relative/path")]
    [InlineData("/relative/{param}/path", "param", "value with spaces", "/relative/value%20with%20spaces/path")]
    [InlineData("/relative/{param}/path", "param", "value/with/slash", "/relative/value%2Fwith%2Fslash/path")]
    public void ShouldModifyPathCorrectly(string origin, string paramName, object value, string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.RelativeOrAbsolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
    }

    [Theory]
    [InlineData("http://example.com/{param}", "param", "value", "http://example.com/value")]
    [InlineData("http://example.com/{param}", "param", null, "http://example.com")]
    [InlineData("http://example.com/{param}", "param", "", "http://example.com")]
    [InlineData("http://example.com/{param}", "param", "value with spaces", "http://example.com/value%20with%20spaces")]
    [InlineData("http://example.com/{param}", "param", "value/with/slash", "http://example.com/value%2Fwith%2Fslash")]
    public void ShouldHandleAbsoluteUris(string origin, string paramName, object value, string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.Absolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
    }

    [Theory]
    [InlineData("/relative/{param}", "param", "value", "/relative/value")]
    [InlineData("/relative/{param}", "param", null, "/relative")]
    [InlineData("/relative/{param}", "param", "", "/relative")]
    [InlineData("/relative/{param}", "param", "value with spaces", "/relative/value%20with%20spaces")]
    [InlineData("/relative/{param}", "param", "value/with/slash", "/relative/value%2Fwith%2Fslash")]
    public void ShouldHandleRelativeUris(string origin, string paramName, object value, string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.Relative);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
    }

    [Theory]
    [MemberData(nameof(GetVariousValueTypes))]
    public void ShouldHandleVariousValueTypes(string origin, string paramName, object value, string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.Absolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
    }

    [Fact]
    public void ShouldThrowForNullUri()
    {
        // Arrange
        var injector = new UrlPathInjector();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => injector.Modify(null, "param", "value"));
    }

    [Fact]
    public void ShouldThrowForNullParamName()
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri("http://example.com/{param}", UriKind.Absolute);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => injector.Modify(uri, null, "value"));
    }

    public static object[][] GetVariousValueTypes()
    {
        return new object[][]
        {
            ["http://example.com/{param}", "param", 123, "http://example.com/123"],
            ["http://example.com/{param}", "param", 123.45, "http://example.com/123.45"],
            ["http://example.com/{param}", "param", true, "http://example.com/True"],
            ["http://example.com/{param}", "param", false, "http://example.com/False"],
            [
                "http://example.com/{param}", "param", new[] { "a", "b", "c" }, "http://example.com/a%2Cb%2Cc"
            ],
            [
                "http://example.com/{param}", "param", new DateTime(2023, 10, 1, 12, 0, 0),
                "http://example.com/2023-10-01T12%3A00%3A00"
            ],
        };
    }

    static string UriToString(Uri uri)
    {
        return uri.OriginalString.Replace(":80", "").TrimEnd('/');
    }
}