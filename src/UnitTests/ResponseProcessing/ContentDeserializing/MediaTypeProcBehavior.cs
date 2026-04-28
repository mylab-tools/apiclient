using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.ResponseProcessing.ContentDeserializing;
using Xunit;

namespace UnitTests.ResponseProcessing.ContentDeserializing;

[TestSubject(typeof(MediaTypeProc))]
public class MediaTypeProcBehavior
{
    [Theory]
    [InlineData("application/json", true)]
    [InlineData("text/plain", true)]
    [InlineData("application/xml", false)]
    [InlineData(null, false)]
    public async Task ShouldReturnCorrectResultBasedOnMediaType(string? mediaType, bool isSupported)
    {
        // Arrange
        var content = new StringContent("");
        content.Headers.ContentType = mediaType != null
            ? new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType)
            : null;

        var proc = new MediaTypeProc(content)
            .Supports("application/json", async c => await Task.FromResult<object?>("json-result"))
            .Supports("text/plain", async c => await Task.FromResult<object?>("text-result"));

        // Act & Assert
        if (isSupported)
        {
            var result = await proc.GetResultAsync();
            Assert.NotNull(result);
        }
        else
        {
            await Assert.ThrowsAsync<UnexpectedResponseContentTypeException>(() => proc.GetResultAsync());
        }
    }

    [Fact]
    public async Task ShouldUseDefaultCreatorWhenMediaTypeIsNotSupported()
    {
        // Arrange
        var content = new StringContent("");
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

        var proc = new MediaTypeProc(content)
            .Supports("application/json", async c => await Task.FromResult<object?>("json-result"))
            .Default(async c => await Task.FromResult<object?>("default-result"));

        // Act
        var result = await proc.GetResultAsync();

        // Assert
        Assert.Equal("default-result", result);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenNoDefaultCreatorAndMediaTypeIsNotSupported()
    {
        // Arrange
        var content = new StringContent("");
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/xml");

        var proc = new MediaTypeProc(content)
            .Supports("application/json", async c => await Task.FromResult<object?>("json-result"));

        // Act & Assert
        await Assert.ThrowsAsync<UnexpectedResponseContentTypeException>(() => proc.GetResultAsync());
    }

    [Fact]
    public void ShouldCreateNewInstanceWhenAddingSupport()
    {
        // Arrange
        var content = new StringContent("");
        var proc = new MediaTypeProc(content);

        // Act
        var newProc = proc.Supports("application/json", async c => await Task.FromResult<object?>("json-result"));

        // Assert
        Assert.NotSame(proc, newProc);
    }

    [Fact]
    public void ShouldCreateNewInstanceWhenSettingDefault()
    {
        // Arrange
        var content = new StringContent("");
        var proc = new MediaTypeProc(content);

        // Act
        var newProc = proc.Default(async c => await Task.FromResult<object?>("default-result"));

        // Assert
        Assert.NotSame(proc, newProc);
    }
}