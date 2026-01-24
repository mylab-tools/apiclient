using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.ContentFactoring;

[TestSubject(typeof(StringHttpContentFactory))]
public class StringHttpContentFactoryBehavior
{
    [Theory]
    [MemberData(nameof(GetValidStringRepresentationCases))]
    public async Task Should_CreateHttpContent_With_ValidStringRepresentation(object? source, string expectedContent)
    {
        // Arrange
        var factory = new StringHttpContentFactory();

        // Act
        var content = factory.Create(source, null);

        // Assert
        Assert.NotNull(content);
        Assert.IsType<StringContent>(content);
        var stringContent = (StringContent)content;
        var actualContent = await stringContent.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(expectedContent, actualContent);
    }

    [Fact]
    public void Should_CreateHttpContent_With_TextMimeType()
    {
        // Arrange
        var factory = new StringHttpContentFactory();

        // Act
        var content = factory.Create("content", null);

        // Assert
        Assert.NotNull(content);
        Assert.NotNull(content.Headers.ContentType);
        Assert.Equal("text/plain", content.Headers.ContentType.MediaType);
    }

    [Fact]
    public void Should_CreateHttpContent_With_ContentLength()
    {
        // Arrange
        var factory = new StringHttpContentFactory();
        const string contentValue = "content"; 
        
        // Act
        var content = factory.Create(contentValue, null);

        // Assert
        Assert.NotNull(content);
        Assert.NotNull(content.Headers.ContentLength);
        Assert.True(content.Headers.ContentLength.HasValue);
        Assert.Equal(contentValue.Length, content.Headers.ContentLength.Value);
    }

    [Fact]
    public async Task Should_Handle_ComplexObjects()
    {
        // Arrange
        var factory = new StringHttpContentFactory();
        var complexObject = new { Name = "Test", Value = 123 };

        // Act
        var result = factory.Create(complexObject, null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        var stringContent = (StringContent)result;
        var actualContent = await stringContent.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(complexObject.ToString(), actualContent);
    }

    [Fact]
    public async Task Should_Handle_EnumerableObjects()
    {
        // Arrange
        var factory = new StringHttpContentFactory();
        var enumerableObject = new List<int> { 1, 2, 3 };

        // Act
        var result = factory.Create(enumerableObject, null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StringContent>(result);
        var stringContent = (StringContent)result;
        var actualContent = await stringContent.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal("1,2,3", actualContent);
    }

    public static IEnumerable<object?[]> GetValidStringRepresentationCases()
    {
        yield return [null, ""];
        yield return ["", ""];
        yield return ["test-string", "test-string"];
        yield return [123, "123"];
        yield return [123.45, "123.45"];
        yield return [true, "True"];
        yield return [false, "False"];
        yield return [DateTime.Parse("2023-10-01T12:34:56"), "2023-10-01T12:34:56"];
        yield return [Guid.Empty, "00000000000000000000000000000000"];
    }
}