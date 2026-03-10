using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring;
using MyLab.ApiClient.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Moq;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring.UrlModifying;
using Xunit;

namespace MyLab.ApiClient.Tests.RequestFactoring;

[TestSubject(typeof(RequestFactory))]
public class RequestFactoryBehavior
{
    [Theory]
    [InlineData(null)]
    public void ShouldThrowArgumentNullExceptionWhenParametersAreNull(object[] parameters)
    {
        // Arrange
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>());
        var epDesc = new EndpointModel(HttpMethod.Get, Array.Empty<IRequestParameterModel>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => factory.Create(parameters));
    }

    [Fact]
    public void ShouldThrowInvalidApiContractExceptionWhenParameterCountMismatch()
    {
        // Arrange
        var epDesc = new EndpointModel(HttpMethod.Get, new[]
            {
                new UrlParameterModel(0, "param1", new UrlQueryInjector())
            });
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act & Assert
        Assert.Throws<InvalidApiContractException>(() => factory.Create(new object[0]));
    }

    [Fact]
    public void ShouldCreateRequestWithCorrectUri()
    {
        // Arrange
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>())
        {
            Url = "/example_com"
        };
        var epDesc = new EndpointModel(HttpMethod.Get, Array.Empty<IRequestParameterModel>())
        {
            Url = "api/test"
        };
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(new Uri("/example_com/api/test", UriKind.Relative), request.RequestUri);
    }

    [Fact]
    public void ShouldCreateRequestWithCorrectHttpMethod()
    {
        // Arrange
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>());
        var epDesc = new EndpointModel(HttpMethod.Post, Array.Empty<IRequestParameterModel>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(HttpMethod.Post, request.Method);
    }

    [Fact]
    public void ShouldApplyParametersCorrectly()
    {
        // Arrange
        var parameterDescription = new Mock<IRequestParameterModel>();
        parameterDescription.Setup(p => p.Position).Returns(0);
        parameterDescription.Setup(p => p.Apply(It.IsAny<HttpRequestMessage>(), It.IsAny<object>()));

        var epDesc = new EndpointModel(HttpMethod.Get, new[]
        {
            parameterDescription.Object
        });
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(new object[] { "value" });

        // Assert
        parameterDescription.Verify(p => p.Apply(It.IsAny<HttpRequestMessage>(), "value"), Times.Once);
    }

    [Fact]
    public void ShouldHandleEmptyServiceUrl()
    {
        // Arrange
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>());
        var epDesc = new EndpointModel(HttpMethod.Get, Array.Empty<IRequestParameterModel>())
        {
            Url = "api/test"
        };
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(new Uri("api/test", UriKind.Relative), request.RequestUri);
    }

    [Fact]
    public void ShouldHandleEmptyEndpointUrl()
    {
        // Arrange
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>())
        {
            Url = "/example_com"
        };
        var epDesc = new EndpointModel(HttpMethod.Get, Array.Empty<IRequestParameterModel>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(new Uri("/example_com/", UriKind.Relative), request.RequestUri);
    }

    [Fact]
    public void ShouldHandleBothEmptyUrls()
    {
        // Arrange
        var serviceDesc = new ServiceModel(new Dictionary<int, EndpointModel>());
        var epDesc = new EndpointModel(HttpMethod.Get, Array.Empty<IRequestParameterModel>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(new Uri("/", UriKind.Relative), request.RequestUri);
    }
}