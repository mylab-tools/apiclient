using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.Options;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.ContentFactoring;

[TestSubject(typeof(UrlFormHttpContentFactory))]
public class UrlFormHttpContentFactoryBehavior
{
    [Theory]
    [MemberData(nameof(ContentCreationCases))]
    public async Task Should_Create_HttpContent_Correctly(object? source, bool[]? settings, string expectedContent)
    {
        // Arrange
        var factory = new UrlFormHttpContentFactory();
        var factoringSettings = settings != null
            ? new RequestFactoringSettings
            {
                UrlFormSettings = new ApiUrlFormSettings
                {
                    EscapeSymbols = settings[0],
                    IgnoreNullValues = settings.Length > 1 && settings[1]
                }
            }
            : null;

        // Act
        var result = factory.Create(source, factoringSettings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        Assert.Equal("application/x-www-form-urlencoded", result.Headers.ContentType?.MediaType);
        Assert.Equal(expectedContent, await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Should_Handle_Empty_Source()
    {
        // Arrange
        var factory = new UrlFormHttpContentFactory();

        // Act
        var result = factory.Create(null, null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        Assert.Equal("application/x-www-form-urlencoded", result.Headers.ContentType?.MediaType);
        Assert.Equal(string.Empty, await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Should_Handle_Null_Values_When_IgnoreNullValues_Is_False()
    {
        // Arrange
        var factory = new UrlFormHttpContentFactory();
        var source = new { Name = "Test", Value = (string?)null };
        var settings = new RequestFactoringSettings
        {
            UrlFormSettings = new ApiUrlFormSettings
            {
                IgnoreNullValues = false
            }
        };

        // Act
        var result = factory.Create(source, settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        Assert.Equal("application/x-www-form-urlencoded", result.Headers.ContentType?.MediaType);
        Assert.Equal("Name=Test&Value=", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Should_Handle_Null_Values_When_IgnoreNullValues_Is_True()
    {
        // Arrange
        var factory = new UrlFormHttpContentFactory();
        var source = new { Name = "Test", Value = (string?)null };
        var settings = new RequestFactoringSettings
        {
            UrlFormSettings = new ApiUrlFormSettings
            {
                IgnoreNullValues = true
            }
        };

        // Act
        var result = factory.Create(source, settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        Assert.Equal("application/x-www-form-urlencoded", result.Headers.ContentType?.MediaType);
        Assert.Equal("Name=Test", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Should_Escape_Symbols_When_EscapeSymbols_Is_True()
    {
        // Arrange
        var factory = new UrlFormHttpContentFactory();
        var source = new { Name = "Test", Value = "Value With Spaces" };
        var settings = new RequestFactoringSettings
        {
            UrlFormSettings = new ApiUrlFormSettings
            {
                EscapeSymbols = true
            }
        };

        // Act
        var result = factory.Create(source, settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        Assert.Equal("application/x-www-form-urlencoded", result.Headers.ContentType?.MediaType);
        Assert.Equal("Name=Test&Value=Value%20With%20Spaces", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Should_Not_Escape_Symbols_When_EscapeSymbols_Is_False()
    {
        // Arrange
        var factory = new UrlFormHttpContentFactory();
        var source = new { Name = "Test", Value = "Value With Spaces" };
        var settings = new RequestFactoringSettings
        {
            UrlFormSettings = new ApiUrlFormSettings
            {
                EscapeSymbols = false
            }
        };

        // Act
        var result = factory.Create(source, settings);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        Assert.Equal("application/x-www-form-urlencoded", result.Headers.ContentType?.MediaType);
        Assert.Equal("Name=Test&Value=Value With Spaces", await result.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    public static IEnumerable<object[]> ContentCreationCases()
    {
        yield return [null, null, ""];
        yield return [null, new [] { true, false }, ""];
        yield return [new object(), null, ""];
        yield return [new { Name = "Test", Value = "Value" }, null, "Name=Test&Value=Value"];
        yield return [new { Name = "Test", Value = (object)null }, new [] { true /*no matter in this case*/, false }, "Name=Test&Value="];
        yield return [new { Name = "Test", Value = (object)null }, new [] { true /*no matter in this case*/, true }, "Name=Test"];
        yield return [new { Name = "Test", Value = "Value" }, new [] { true }, "Name=Test&Value=Value"];
        yield return [new { Name = "Test", Value = "Value" }, new [] { false }, "Name=Test&Value=Value"];
        yield return [new { Name = "Test", Value = "Value With Spaces" }, new [] { true }, "Name=Test&Value=Value%20With%20Spaces"
        ];
        yield return [new { Name = "Test", Value = "Value With Spaces" }, new [] { false }, "Name=Test&Value=Value With Spaces"
        ];
    }
}