using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(UShortContentDeserializer))]
public class UShortContentDeserializerBehavior
{
    readonly UShortContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidUShortFromJsonContent()
    {
        // Arrange
        var content = new StringContent("12345", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(ushort));
        // Assert
        Assert.Equal((ushort)12345, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidUShortContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(ushort)));
    }
}