using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(DecimalContentDeserializer))]
public class DecimalContentDeserializerBehavior
{
    readonly DecimalContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidDecimalFromJsonContent()
    {
        // Arrange
        var content = new StringContent("123.45", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(decimal));
        // Assert
        Assert.Equal(123.45m, result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidDecimalContent()
    {
        // Arrange
        var content = new StringContent("\"invalid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(decimal)));
    }
}