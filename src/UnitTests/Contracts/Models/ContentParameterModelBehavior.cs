using System;
using System.Net.Http;
using JetBrains.Annotations;
using Moq;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts.Models;

[TestSubject(typeof(ContentParameterModel))]
public class ContentParameterModelBehavior
{
    [Fact]
    public void ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var position = 1;
        var contentFactoryMock = new Mock<IHttpContentFactory>();

        // Act
        var description = new ContentParameterModel(position, contentFactoryMock.Object);

        // Assert
        Assert.Equal(position, description.Position);
        Assert.Equal(contentFactoryMock.Object, description.ContentFactory);
    }

    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenRequestIsNullInApply()
    {
        // Arrange
        var contentFactoryMock = new Mock<IHttpContentFactory>();
        var description = new ContentParameterModel(1, contentFactoryMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => description.Apply(null, new object()));
    }

    [Fact]
    public void ShouldSetRequestContentUsingContentFactory()
    {
        // Arrange
        var contentFactoryMock = new Mock<IHttpContentFactory>();
        var httpContentMock = new Mock<HttpContent>();
        contentFactoryMock
            .Setup(cf => cf.Create(It.IsAny<object>(), It.IsAny<RequestFactoringSettings>()))
            .Returns(httpContentMock.Object);

        var description = new ContentParameterModel(1, contentFactoryMock.Object);
        var request = new HttpRequestMessage();
        var value = new object();

        // Act
        description.Apply(request, value);

        // Assert
        Assert.Equal(httpContentMock.Object, request.Content);
        contentFactoryMock.Verify(cf => cf.Create(value, null), Times.Once);
    }

    [Fact]
    public void ShouldHandleNullValueInApply()
    {
        // Arrange
        var contentFactoryMock = new Mock<IHttpContentFactory>();
        var httpContentMock = new Mock<HttpContent>();
        contentFactoryMock
            .Setup(cf => cf.Create(null, It.IsAny<RequestFactoringSettings>()))
            .Returns(httpContentMock.Object);

        var description = new ContentParameterModel(1, contentFactoryMock.Object);
        var request = new HttpRequestMessage();

        // Act
        description.Apply(request, null);

        // Assert
        Assert.Equal(httpContentMock.Object, request.Content);
        contentFactoryMock.Verify(cf => cf.Create(null, null), Times.Once);
    }
}