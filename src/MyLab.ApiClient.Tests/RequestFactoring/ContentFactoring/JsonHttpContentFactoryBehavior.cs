using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.ContentFactoring;

[TestSubject(typeof(JsonHttpContentFactory))]
public class JsonHttpContentFactoryBehavior
{
    [Theory]
    [InlineData(null)]
    [InlineData(typeof(object))]
    public async Task ShouldCreateHttpContentWithProperSerialization(Type? sourceType)
    {
        // Arrange
        object? source = sourceType == null ? null : new { Name = "Test", Value = 123 };

        var factory = new JsonHttpContentFactory();

        // Act
        var content = factory.Create(source, null);

        // Assert
        Assert.NotNull(content);
        Assert.IsType<StringContent>(content);
        Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
        if (source != null)
        {
            var serialized = JsonConvert.SerializeObject(source);
            Assert.Equal(serialized, await content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        }
    }

    [Fact]
    public async Task ShouldHandleNullSourceGracefully()
    {
        // Arrange
        var factory = new JsonHttpContentFactory();

        // Act
        var content = factory.Create(null, null);

        // Assert
        Assert.NotNull(content);
        Assert.IsType<StringContent>(content);
        Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
        Assert.Equal("null", await content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ShouldHandleEmptyJsonSettings()
    {
        // Arrange
        var factory = new JsonHttpContentFactory();
        var settings = new RequestFactoringSettings
        {
            JsonSettings = new JsonSerializerSettings()
        };

        // Act
        var content = factory.Create(new { Name = "Test" }, settings);

        // Assert
        Assert.NotNull(content);
        Assert.IsType<StringContent>(content);
        Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
        Assert.Equal("{\"Name\":\"Test\"}", await content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ShouldCalculateContentLength()
    {
        // Arrange
        var factory = new JsonHttpContentFactory();
        var source = new { Name = "Test", Value = 123 };

        // Act
        var content = factory.Create(source, null);

        // Assert
        Assert.NotNull(content);
        Assert.True(content.Headers.ContentLength > 0);
    }

    [Fact]
    public void ShouldSetDefaultContentType()
    {
        // Arrange
        var factory = new JsonHttpContentFactory();
        var source = new { Name = "Test" };

        // Act
        var content = factory.Create(source, null);

        // Assert
        Assert.NotNull(content);
        Assert.Equal("application/json", content.Headers.ContentType?.MediaType);
    }
}