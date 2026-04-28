using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(DoubleContentDeserializer))]
public class DoubleContentDeserializerBehavior
{
    readonly DoubleContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidDoubleFromJsonContent()
    {
        // Arrange
        var content = new StringContent("123.45", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(double));
        // Assert
        Assert.Equal(123.45, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidDoubleContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(double)));
    }
}