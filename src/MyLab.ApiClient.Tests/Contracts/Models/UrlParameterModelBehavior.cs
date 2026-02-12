using System;
using System.Net.Http;
using JetBrains.Annotations;
using Moq;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring.UrlModifying;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts.Models;

[TestSubject(typeof(UrlParameterModel))]
public class UrlParameterModelBehavior
{

    [Fact]
    public void ShouldThrowIfRequestIsNull()
    {
        // Arrange
        var mockModifier = new Mock<IUrlModifier>();
        var description = new UrlParameterModel(0, "param", mockModifier.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => description.Apply(null, "value"));
    }

    [Fact]
    public void ShouldModifyRequestUri()
    {
        // Arrange
        var mockModifier = new Mock<IUrlModifier>();
        var description = new UrlParameterModel(0, "param", mockModifier.Object);
        var request = new HttpRequestMessage { RequestUri = new Uri("http://example.com") };
        var modifiedUri = new Uri("/example/modified", UriKind.Relative);

        mockModifier
            .Setup(m => m.Modify(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<object>()))
            .Returns(modifiedUri);

        // Act
        description.Apply(request, "value");

        // Assert
        Assert.Equal(modifiedUri, request.RequestUri);
    }

    [Fact]
    public void ShouldHandleNullRequestUri()
    {
        // Arrange
        var mockModifier = new Mock<IUrlModifier>();
        var description = new UrlParameterModel(0, "param", mockModifier.Object);
        var request = new HttpRequestMessage { RequestUri = null };
        var modifiedUri = new Uri("/example/modified", UriKind.Relative);

        mockModifier
            .Setup(m => m.Modify(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<object>()))
            .Returns(modifiedUri);

        // Act
        description.Apply(request, "value");

        // Assert
        Assert.Equal(modifiedUri, request.RequestUri);
    }
}