using JetBrains.Annotations;
using MyLab.ApiClient.Contracts.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts.Descriptions;

[TestSubject(typeof(HeaderCollectionParameterDescription))]
public class HeaderCollectionParameterDescriptionBehavior
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void ShouldInitializePositionCorrectly(int position)
    {
        // Act
        var description = new HeaderCollectionParameterDescription(position);

        // Assert
        Assert.Equal(position, description.Position);
    }

    [Fact]
    public void ShouldThrowIfRequestIsNull()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => description.Apply(null!, new List<KeyValuePair<string, object>>()));
    }

    [Fact]
    public void ShouldThrowIfValueIsNotEnumerableOfKeyValuePairs()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);
        var request = new HttpRequestMessage();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => description.Apply(request, "invalid-value"));
    }

    [Fact]
    public void ShouldAddHeadersToRequest()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);
        var request = new HttpRequestMessage();
        var headers = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("Header1", "Value1"),
            new KeyValuePair<string, object>("Header2", 123),
            new KeyValuePair<string, object>("Header3", null)
        };

        // Act
        description.Apply(request, headers);

        // Assert
        Assert.Equal("Value1", request.Headers.GetValues("Header1").First());
        Assert.Equal("123", request.Headers.GetValues("Header2").First());
        Assert.Equal(string.Empty, request.Headers.GetValues("Header3").First());
    }

    [Fact]
    public void ShouldHandleEmptyHeaders()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);
        var request = new HttpRequestMessage();
        var headers = new List<KeyValuePair<string, object>>();

        // Act
        description.Apply(request, headers);

        // Assert
        Assert.Empty(request.Headers);
    }

    [Fact]
    public void ShouldHandleNullHeaderValues()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);
        var request = new HttpRequestMessage();
        var headers = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("Header1", null)
        };

        // Act
        description.Apply(request, headers);

        // Assert
        Assert.Equal(string.Empty, request.Headers.GetValues("Header1").First());
    }

    [Fact]
    public void ShouldHandleSpecialCharactersInHeaderValues()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);
        var request = new HttpRequestMessage();
        var headers = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("Header1", "Value!@#$%^&*()")
        };

        // Act
        description.Apply(request, headers);

        // Assert
        Assert.Equal("Value!@#$%^&*()", request.Headers.GetValues("Header1").First());
    }

    [Fact]
    public void ShouldHandleEnumerableHeaderValues()
    {
        // Arrange
        var description = new HeaderCollectionParameterDescription(0);
        var request = new HttpRequestMessage();
        var headers = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("Header1", new[] { "Value1", "Value2" })
        };

        // Act
        description.Apply(request, headers);

        // Assert
        Assert.Equal("Value1,Value2", request.Headers.GetValues("Header1").First());
    }
}