using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MyLab.ApiClient.Tests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(DateTimeContentDeserializer))]
public class DateTimeContentDeserializerBehavior
{
    readonly DateTimeContentDeserializer _deserializer = new();

    [Fact]
    public async Task ShouldThrowArgumentNullExceptionWhenContentIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _deserializer.DeserializeAsync(null, typeof(DateTime)));
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
    public async Task ShouldDeserializeValidDateTimeFromJsonContent()
    {
        // Arrange
        var validDateTime = "2023-10-05T14:48:00Z";
        var content = new StringContent($"\"{validDateTime}\"", Encoding.UTF8, "application/json");
        // Act
        var result = await _deserializer.DeserializeAsync(content, typeof(DateTime));
        // Assert
        Assert.Equal(DateTime.Parse(validDateTime), result);
    }
    [Fact]
    public async Task ShouldThrowFormatExceptionForInvalidDateTimeContent()
    {
        // Arrange
        var invalidDateTime = "invalid-date";
        var content = new StringContent($"\"{invalidDateTime}\"", Encoding.UTF8, "application/json");
        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() =>
            _deserializer.DeserializeAsync(content, typeof(DateTime)));
    }
    [Fact]
    public async Task ShouldThrowUnexpectedResponseContentTypeExceptionForUnsupportedMediaType()
    {
        // Arrange
        var validDateTime = "2023-10-05T14:48:00Z";
        var content = new StringContent($"\"{validDateTime}\"", Encoding.UTF8, "unsupported/type");
        // Act & Assert
        await Assert.ThrowsAsync<UnexpectedResponseContentTypeException>(() =>
            _deserializer.DeserializeAsync(content, typeof(DateTime)));
    }
}