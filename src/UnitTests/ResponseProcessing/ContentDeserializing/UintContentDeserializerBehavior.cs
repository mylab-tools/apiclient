using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(UintContentDeserializer))]
public class UintContentDeserializerBehavior
{
    readonly UintContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidUintFromJsonContent()
    {
        // Arrange
        var content = new StringContent("123", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(uint));
        // Assert
        Assert.Equal((uint)123, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidUintContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(uint)));
    }
}