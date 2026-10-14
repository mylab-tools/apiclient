using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Contracts.Models;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using MyLab.ApiClient.Tools;
using Xunit;

namespace UnitTests.Tools;

[TestSubject(typeof(RequestParameterModelExtractor))]
public class RequestParameterModelExtractorBehavior
{
    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenMethodInfoIsNull()
    {
        // Arrange
        MethodInfo? methodInfo = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => RequestParameterModelExtractor.FromMethod(methodInfo!));
    }

    [Fact]
    public void ShouldExtractUrlParameterModel()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithUrlParameter));
        var settings = new RequestFactoringSettings();

        // Act
        var result = RequestParameterModelExtractor.FromMethod(methodInfo!, settings).ToArray();

        // Assert
        Assert.Single(result);
        Assert.IsType<UrlParameterModel>(result[0]);
        var urlParam = (UrlParameterModel)result[0];
        Assert.Equal(0, urlParam.Position);
        Assert.Equal("param", urlParam.Name);
        Assert.NotNull(urlParam.Modifier);
        Assert.Equal(settings, urlParam.Settings);
    }

    [Fact]
    public void ShouldExtractHeaderParameterModel()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithHeaderParameter));
        var settings = new RequestFactoringSettings();

        // Act
        var result = RequestParameterModelExtractor.FromMethod(methodInfo!, settings).ToArray();

        // Assert
        Assert.Single(result);
        Assert.IsType<HeaderParameterModel>(result[0]);
        var headerParam = (HeaderParameterModel)result[0];
        Assert.Equal(0, headerParam.Position);
        Assert.Equal("header", headerParam.Name);
        Assert.Equal(settings, headerParam.Settings);
    }

    [Fact]
    public void ShouldExtractHeaderCollectionParameterModel()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithHeaderCollectionParameter));
        var settings = new RequestFactoringSettings();

        // Act
        var result = RequestParameterModelExtractor.FromMethod(methodInfo!, settings).ToArray();

        // Assert
        Assert.Single(result);
        Assert.IsType<HeaderCollectionParameterModel>(result[0]);
        var headerCollectionParam = (HeaderCollectionParameterModel)result[0];
        Assert.Equal(0, headerCollectionParam.Position);
        Assert.Equal(settings, headerCollectionParam.Settings);
    }

    [Fact]
    public void ShouldExtractContentParameterModel()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithContentParameter));
        var settings = new RequestFactoringSettings();

        // Act
        var result = RequestParameterModelExtractor.FromMethod(methodInfo!, settings).ToArray();

        // Assert
        Assert.Single(result);
        Assert.IsType<ContentParameterModel>(result[0]);
        var contentParam = (ContentParameterModel)result[0];
        Assert.Equal(0, contentParam.Position);
        Assert.NotNull(contentParam.ContentFactory);
        Assert.Equal(settings, contentParam.Settings);
    }

    [Fact]
    public void ShouldThrowInvalidApiContractExceptionForUnsupportedAttribute()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithUnsupportedParameter));

        // Act & Assert
        Assert.Throws<InvalidApiContractException>(() => RequestParameterModelExtractor.FromMethod(methodInfo!));
    }

    [Fact]
    public void ShouldThrowInvalidApiContractExceptionForMissingAttribute()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithoutAttribute));

        // Act & Assert
        Assert.Throws<InvalidApiContractException>(() => RequestParameterModelExtractor.FromMethod(methodInfo!));
    }

    [Fact]
    public void ShouldThrowInvalidApiContractExceptionForMultipleAttributes()
    {
        // Arrange
        var methodInfo = typeof(ITestApi).GetMethod(nameof(ITestApi.MethodWithMultipleAttributes));

        // Act & Assert
        Assert.Throws<InvalidApiContractException>(() => RequestParameterModelExtractor.FromMethod(methodInfo!));
    }

    interface ITestApi
    {
        void MethodWithUrlParameter([Path("param")] string param);

        void MethodWithHeaderParameter([Header("header")] string header);

        void MethodWithHeaderCollectionParameter([HeaderCollection] IDictionary<string, object> headers);

        void MethodWithContentParameter([JsonContent] object content);

        void MethodWithUnsupportedParameter([Unsupported] string unsupported);

        void MethodWithoutAttribute(string noAttr);

        void MethodWithMultipleAttributes([Path("param"), Header("header")] string multiple);
    }

    class UnsupportedAttribute : ApiParameterAttribute
    {
        
    }
}