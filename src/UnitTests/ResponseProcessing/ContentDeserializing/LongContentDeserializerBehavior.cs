using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(LongContentDeserializer))]
public class LongContentDeserializerBehavior
{
    readonly LongContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidLongFromJsonContent()
    {
        // Arrange
        var content = new StringContent("123456789012345", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(long));
        // Assert
        Assert.Equal(123456789012345L, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidLongContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(long)));
    }
}