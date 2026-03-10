using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace MyLab.ApiClient.Tests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(BoolContentDeserializer))]
public class BoolContentDeserializerBehavior
{
    readonly BoolContentDeserializer _deserializer = new();

    [Fact]
    public async Task ShouldThrowArgumentNullExceptionWhenContentIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deserializer.DeserializeAsync(null, typeof(bool)));
    }
    
    [Fact]
    public async Task ShouldThrowArgumentNullExceptionWhenReturnTypeIsNull()
    {
        // Arrange
        var content = new StringContent("");
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deserializer.DeserializeAsync(content, null));
    }
    
    [Fact]
    public async Task ShouldDeserializeTrueFromJsonContent()
    {
        // Arrange
        var content = new StringContent("true", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(bool));
        // Assert
        Assert.True((bool)result);
    }
    
    [Fact]
    public async Task ShouldDeserializeFalseFromJsonContent()
    {
        // Arrange
        var content = new StringContent("false", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(bool));
        // Assert
        Assert.False((bool)result);
    }
    
    [Fact]
    public async Task ShouldThrowUnexpectedResponseContentTypeExceptionForUnsupportedMediaType()
    {
        // Arrange
        var content = new StringContent("true", Encoding.UTF8, "unsupported/type");
        // Act & Assert
        await Assert.ThrowsAsync<UnexpectedResponseContentTypeException>(() =>
            _deserializer.DeserializeAsync(content, typeof(bool)));
    }
}