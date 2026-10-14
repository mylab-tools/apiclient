using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(TimeSpanContentDeserializer))]
public class TimeSpanContentDeserializerBehavior
{
    readonly TimeSpanContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidTimeSpanFromJsonContent()
    {
        // Arrange
        var content = new StringContent("\"00:30:00\"", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(TimeSpan));
        // Assert
        Assert.Equal(TimeSpan.FromMinutes(30), result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidTimeSpanContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(TimeSpan)));
    }
}