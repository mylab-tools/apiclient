using JetBrains.Annotations;
using MyLab.ApiClient.RequestFactoring;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Descriptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Moq;
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
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>());
        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => factory.Create(parameters));
    }

    [Fact]
    public void ShouldThrowInvalidApiContractExceptionWhenParameterCountMismatch()
    {
        // Arrange
        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions
        {
            UrlParams = new List<UrlParameterDescription>
            {
                new UrlParameterDescription(0, "param1", null)
            }
        });
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act & Assert
        Assert.Throws<InvalidApiContractException>(() => factory.Create(new object[0]));
    }

    [Fact]
    public void ShouldCreateRequestWithCorrectUri()
    {
        // Arrange
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>())
        {
            Url = "http://example.com"
        };
        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions())
        {
            Url = "api/test"
        };
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(new Uri("http://example.com/api/test"), request.RequestUri);
    }

    [Fact]
    public void ShouldCreateRequestWithCorrectHttpMethod()
    {
        // Arrange
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>());
        var epDesc = new EndpointDescription(HttpMethod.Post, new RequestParametersDescriptions());
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
        var parameterDescription = new Mock<IRequestParameterDescription>();
        parameterDescription.Setup(p => p.Position).Returns(0);
        parameterDescription.Setup(p => p.Apply(It.IsAny<HttpRequestMessage>(), It.IsAny<object>()));

        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions
        {
            UrlParams = new List<UrlParameterDescription>
            {
                new UrlParameterDescription(0, "param1", null)
            }
        });
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>());
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
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>());
        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions())
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
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>())
        {
            Url = "http://example.com"
        };
        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Equal(new Uri("http://example.com"), request.RequestUri);
    }

    [Fact]
    public void ShouldHandleBothEmptyUrls()
    {
        // Arrange
        var serviceDesc = new ServiceDescription(new Dictionary<int, EndpointDescription>());
        var epDesc = new EndpointDescription(HttpMethod.Get, new RequestParametersDescriptions());
        var factory = new RequestFactory(serviceDesc, epDesc);

        // Act
        var request = factory.Create(Array.Empty<object>());

        // Assert
        Assert.Null(request.RequestUri);
    }
}