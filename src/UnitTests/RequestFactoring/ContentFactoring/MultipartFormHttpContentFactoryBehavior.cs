using System;
using System.Net.Http;
using JetBrains.Annotations;
using Moq;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring.ContentFactoring;

[TestSubject(typeof(MultipartFormHttpContentFactory))]
public class MultipartFormHttpContentFactoryBehavior
{
    [Theory]
    [InlineData(null)]
    [InlineData(typeof(IMultipartContentParameter))]
    public void ShouldCreateMultipartFormDataContent(Type? sourceType)
    {
        // Arrange
        object? source = sourceType == typeof(IMultipartContentParameter)
            ? Mock.Of<IMultipartContentParameter>()
            : null;

        var factory = new MultipartFormHttpContentFactory();

        // Act
        var result = factory.Create(source, null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<MultipartFormDataContent>(result);

        if (source is IMultipartContentParameter parameter)
        {
            Mock.Get(parameter).Verify(p => p.AddParts(It.IsAny<MultipartFormDataContent>()), Times.Once);
        }
    }

    [Fact]
    public void ShouldHandleNullSourceGracefully()
    {
        // Arrange
        var factory = new MultipartFormHttpContentFactory();

        // Act
        var result = factory.Create(null, null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<MultipartFormDataContent>(result);
    }

    [Fact]
    public void ShouldInvokeAddPartsWhenSourceImplementsIMultipartContentParameter()
    {
        // Arrange
        var mockParameter = new Mock<IMultipartContentParameter>();
        var factory = new MultipartFormHttpContentFactory();

        // Act
        var result = factory.Create(mockParameter.Object, null);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<MultipartFormDataContent>(result);
        mockParameter.Verify(p => p.AddParts(It.IsAny<MultipartFormDataContent>()), Times.Once);
    }
}