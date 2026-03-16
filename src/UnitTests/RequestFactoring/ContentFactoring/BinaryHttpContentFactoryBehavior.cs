using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.ContentFactoring;

[TestSubject(typeof(BinaryHttpContentFactory))]
public class BinaryHttpContentFactoryBehavior
{
    [Theory]
    [InlineData(null)]
    public void ShouldThrowArgumentNullExceptionWhenSourceIsNull(object? source)
    {
        // Arrange
        var factory = new BinaryHttpContentFactory();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => factory.Create(source, null));
    }

    [Fact]
    public async Task ShouldCreateByteArrayContentWithDefaultMimeType()
    {
        // Arrange
        var factory = new BinaryHttpContentFactory();
        var source = new byte[] { 1, 2, 3 };

        // Act
        var content = factory.Create(source, null);
        var binContent = await TryExtractBinContentAsync(content);
        
        // Assert
        Assert.IsType<ByteArrayContent>(content);
        Assert.Equal(BinToStr(source), BinToStr(binContent));
        Assert.Equal("application/octet-stream", content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task ShouldCreateByteArrayContentWithCustomMimeType()
    {
        // Arrange
        var mimeType = "image/png";
        var factory = new BinaryHttpContentFactory(mimeType);
        var source = new byte[] { 1, 2, 3 };

        // Act
        var content = factory.Create(source, null);
        var binContent = await TryExtractBinContentAsync(content);

        // Assert
        Assert.IsType<ByteArrayContent>(content);
        Assert.Equal(BinToStr(source), BinToStr(binContent));
        Assert.Equal(mimeType, content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void ShouldThrowNotSupportedExceptionForUnsupportedSourceType()
    {
        // Arrange
        var factory = new BinaryHttpContentFactory();
        var source = "unsupported";

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => factory.Create(source, null));
    }

    [Fact]
    public void ShouldHandleEmptyByteArray()
    {
        // Arrange
        var factory = new BinaryHttpContentFactory();
        var source = Array.Empty<byte>();

        // Act
        var content = factory.Create(source, null);

        // Assert
        Assert.IsType<ByteArrayContent>(content);
        Assert.Equal("application/octet-stream", content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void ShouldHandleNullMimeType()
    {
        // Arrange
        var factory = new BinaryHttpContentFactory(null);
        var source = new byte[] { 1, 2, 3 };

        // Act
        var content = factory.Create(source, null);

        // Assert
        Assert.IsType<ByteArrayContent>(content);
        Assert.Equal("application/octet-stream", content.Headers.ContentType?.MediaType);
    }

    async Task<byte[]?> TryExtractBinContentAsync(HttpContent httpContent)
    {
        if (httpContent is ByteArrayContent bac)
        {
            using var mem = new MemoryStream();
            await bac.CopyToAsync(mem, null, TestContext.Current.CancellationToken);
            return mem.ToArray();
        }

        return null;
    }

    string BinToStr(byte[]? arr)
    {
        return arr != null 
            ? BitConverter.ToString(arr).Replace("-", "")
            : string.Empty;
    }
}