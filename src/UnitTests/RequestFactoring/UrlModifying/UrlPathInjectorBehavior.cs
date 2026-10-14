using System;
using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring.UrlModifying;
using Xunit;

namespace UnitTests.RequestFactoring.UrlModifying;

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

    /// <summary>
    /// Reveals: EscapeDataString does not encode '.' so ".." remains a path segment
    /// and UriBuilder normalizes /a/../b → /b (path traversal).
    /// </summary>
    [Theory]
    [InlineData("http://example.com/a/{param}/b", "param", "..", "http://example.com/a/%2E%2E/b")]
    [InlineData("http://example.com/a/{param}/b", "param", ".", "http://example.com/a/%2E/b")]
    [InlineData("/a/{param}/b", "param", "..", "/a/%2E%2E/b")]
    [InlineData("/a/{param}/b", "param", ".", "/a/%2E/b")]
    public void ShouldNotAllowPathTraversalViaPathParameter(
        string origin,
        string paramName,
        object value,
        string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.RelativeOrAbsolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert — segment must stay encoded so ".." / "." cannot act as path navigation
        Assert.Equal(expected, UriToString(result));
        Assert.DoesNotContain("/../", UriToString(result));
        Assert.DoesNotContain("/./", UriToString(result));
    }

    /// <summary>
    /// Reveals: for absolute URIs PathAndQuery is passed as UriBuilder path argument,
    /// so '?' is encoded into the path and the original query is lost.
    /// </summary>
    [Theory]
    [InlineData(
        "http://example.com/api/{param}?x=1&y=2",
        "param",
        "v",
        "http://example.com/api/v?x=1&y=2")]
    [InlineData(
        "http://example.com/{param}?q=search",
        "param",
        "orders",
        "http://example.com/orders?q=search")]
    [InlineData(
        "http://example.com/api/{param}/details?filter=active",
        "param",
        "42",
        "http://example.com/api/42/details?filter=active")]
    public void ShouldPreserveQueryStringWhenInjectingIntoAbsoluteUri(
        string origin,
        string paramName,
        object value,
        string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.Absolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
        Assert.Equal(new Uri(expected).Query, result.Query);
    }

    /// <summary>
    /// Reveals: UrlDecode of the whole path before injection turns encoded segments
    /// (%2E%2E, %2F) into traversable / structural characters.
    /// </summary>
    [Theory]
    [InlineData(
        "http://example.com/files/%2E%2E/%2E%2E/etc/{param}",
        "param",
        "passwd",
        "http://example.com/files/%2E%2E/%2E%2E/etc/passwd")]
    [InlineData(
        "http://example.com/%2Ffoo/{param}",
        "param",
        "bar",
        "http://example.com/%2Ffoo/bar")]
    [InlineData(
        "/files/%2E%2E/%2E%2E/etc/{param}",
        "param",
        "passwd",
        "/files/%2E%2E/%2E%2E/etc/passwd")]
    [InlineData(
        "/%2Ffoo/{param}",
        "param",
        "bar",
        "/%2Ffoo/bar")]
    public void ShouldNotUrlDecodePathTemplateBeforeInjection(
        string origin,
        string paramName,
        object value,
        string expected)
    {
        // Arrange
        var injector = new UrlPathInjector();
        var uri = new Uri(origin, UriKind.RelativeOrAbsolute);

        // Act
        var result = injector.Modify(uri, paramName, value);

        // Assert
        Assert.Equal(expected, UriToString(result));
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
                "http://example.com/2023-10-01T12%3A00%3A00.0000000"
            ],
        };
    }

    static string UriToString(Uri uri)
    {
        return uri.OriginalString.Replace(":80", "").TrimEnd('/');
    }
}