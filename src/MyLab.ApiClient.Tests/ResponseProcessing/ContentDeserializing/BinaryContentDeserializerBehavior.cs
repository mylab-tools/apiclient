using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace MyLab.ApiClient.Tests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(BinaryContentDeserializer))]
public class BinaryContentDeserializerBehavior
{
    readonly BinaryContentDeserializer _deserializer = new();
    
    [Theory]
    [InlineData(typeof(byte[]), true)]
    [InlineData(typeof(string), false)]
    [InlineData(null, false)]
    public void ShouldCorrectlyPredicateReturnType(Type returnType, bool expectedResult)
    {
        // Act
        var result = _deserializer.Predicate(returnType);
        // Assert
        Assert.Equal(expectedResult, result);
    }
    [Fact]
    public async Task ShouldThrowArgumentNullExceptionWhenContentIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deserializer.DeserializeAsync(null, typeof(byte[])));
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
    public async Task ShouldDeserializeBase64EncodedJsonContent()
    {
        // Arrange
        var base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("test"));
        var content = new StringContent($"\"{base64Encoded}\"", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(byte[]));
        // Assert
        Assert.Equal(Encoding.UTF8.GetBytes("test"), result);
    }
    [Fact]
    public async Task ShouldDeserializeEmptyJsonContent()
    {
        // Arrange
        var content = new StringContent("", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(byte[]));
        // Assert
        Assert.Equal(Array.Empty<byte>(), result);
    }
    [Fact]
    public async Task ShouldDeserializeJsonContentAsByteArray()
    {
        // Arrange
        var content = new StringContent("test", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(byte[]));
        // Assert
        Assert.Equal(Encoding.UTF8.GetBytes("test"), result);
    }
    [Fact]
    public async Task ShouldDeserializeDefaultContentAsByteArray()
    {
        // Arrange
        var content = new StringContent("default content", Encoding.UTF8, "text/plain");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(byte[]));
        // Assert
        Assert.Equal(Encoding.UTF8.GetBytes("default content"), result);
    }
}