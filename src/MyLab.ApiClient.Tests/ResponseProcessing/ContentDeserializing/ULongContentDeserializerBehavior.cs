using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace MyLab.ApiClient.Tests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(ULongContentDeserializer))]
public class ULongContentDeserializerBehavior
{
    readonly ULongContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidULongFromJsonContent()
    {
        // Arrange
        var content = new StringContent("123456789012345", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(ulong));
        // Assert
        Assert.Equal((ulong)123456789012345, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidULongContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(ulong)));
    }
}