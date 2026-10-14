using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(GuidContentDeserializer))]
public class GuidContentDeserializerBehavior
{
    readonly GuidContentDeserializer _deserializer = new();
    
    [Fact]
    public async Task ShouldDeserializeValidGuidFromJsonContent()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var content = new StringContent($"\"{validGuid}\"", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(Guid));
        // Assert
        Assert.Equal(Guid.Parse(validGuid), result);
    }
    
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidGuidContent()
    {
        // Arrange
        var content = new StringContent("\"invalid-guid\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(Guid)));
    }
}