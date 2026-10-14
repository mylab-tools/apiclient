using System;
using System.Net.Http;
using JetBrains.Annotations;
using MyLab.ApiClient.Contracts.Models;
using Xunit;

namespace UnitTests.Contracts.Models;

[TestSubject(typeof(HeaderParameterModel))]
public class HeaderParameterModelBehavior
{
    [Theory]
    [InlineData(0, "HeaderName")]
    [InlineData(1, "AnotherHeader")]
    [InlineData(-1, "NegativePositionHeader")]
    [InlineData(int.MaxValue, "MaxIntHeader")]
    [InlineData(int.MinValue, "MinIntHeader")]
    public void ShouldInitializeCorrectly(int position, string name)
    {
        // Act
        var headerParamDesc = new HeaderParameterModel(position, name);

        // Assert
        Assert.Equal(position, headerParamDesc.Position);
        Assert.Equal(name, headerParamDesc.Name);
    }

    [Fact]
    public void ShouldThrowIfRequestIsNullInApply()
    {
        // Arrange
        var headerParamDesc = new HeaderParameterModel(0, "HeaderName");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => headerParamDesc.Apply(null, "value"));
    }

    [Fact]
    public void ShouldThrowIfValueIsNullInApply()
    {
        // Arrange
        var headerParamDesc = new HeaderParameterModel(0, "HeaderName");
        var request = new HttpRequestMessage();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => headerParamDesc.Apply(request, null));
    }

    [Theory]
    [InlineData("HeaderValue")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData(null)]
    public void ShouldApplyHeaderCorrectly(string value)
    {
        // Arrange
        var headerParamDesc = new HeaderParameterModel(0, "HeaderName");
        var request = new HttpRequestMessage();

        // Act
        if (value == null)
        {
            Assert.Throws<ArgumentNullException>(() => headerParamDesc.Apply(request, value));
        }
        else
        {
            headerParamDesc.Apply(request, value);

            // Assert
            Assert.True(request.Headers.Contains("HeaderName"));
            Assert.Contains(value, request.Headers.GetValues("HeaderName"));
        }
    }

    [Fact]
    public void ShouldHandleComplexObjectValue()
    {
        // Arrange
        var headerParamDesc = new HeaderParameterModel(0, "HeaderName");
        var request = new HttpRequestMessage();
        var complexValue = new { Prop1 = "Value1", Prop2 = 123 };

        // Act
        headerParamDesc.Apply(request, complexValue);

        // Assert
        Assert.True(request.Headers.Contains("HeaderName"));
        Assert.Contains(complexValue.ToString(), request.Headers.GetValues("HeaderName"));
    }

    [Fact]
    public void ShouldHandleEnumerableValue()
    {
        // Arrange
        var headerParamDesc = new HeaderParameterModel(0, "HeaderName");
        var request = new HttpRequestMessage();
        var enumerableValue = new[] { "Value1", "Value2", "Value3" };

        // Act
        headerParamDesc.Apply(request, enumerableValue);

        // Assert
        Assert.True(request.Headers.Contains("HeaderName"));
        Assert.Contains("Value1,Value2,Value3", request.Headers.GetValues("HeaderName"));
    }
}