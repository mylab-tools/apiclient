using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using MyLab.ApiClient.Contracts.Descriptions;
using MyLab.ApiClient.RequestFactoring.ContentFactoring;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts.Descriptions;

[TestSubject(typeof(RequestParametersDescriptions))]
public class RequestParametersDescriptionsBehavior
{
    [Fact]
    public void ShouldProvideUrlParameterDescriptions()
    {
        //Arrange
        var m = GetMethod(nameof(IApiContract.UrlMethod));

        //Act
        var desc = RequestParametersDescriptions.FromMethod(m, null);

        //Assert
        Assert.NotNull(desc.UrlParams);
        Assert.Contains(desc.UrlParams, p => p is { Name: IApiContract.PathParamName, Position: 0 });
        Assert.Contains(desc.UrlParams, p => p is { Name: IApiContract.QueryParamName, Position: 1 });
    }

    [Fact]
    public void ShouldProvideHeaderParameterDescriptions()
    {
        //Arrange
        var m = GetMethod(nameof(IApiContract.HeaderMethod));

        //Act
        var desc = RequestParametersDescriptions.FromMethod(m, null);

        //Assert
        Assert.NotNull(desc.HeaderParams);
        Assert.Contains(desc.HeaderParams, p => p is { Name: IApiContract.HeaderParamName, Position: 0 });
    }

    [Fact]
    public void ShouldProvideContentParameterDescriptions()
    {
        //Arrange
        var m = GetMethod(nameof(IApiContract.ContentMethod));

        //Act
        var desc = RequestParametersDescriptions.FromMethod(m, null);

        //Assert
        Assert.NotNull(desc.ContentParams);
        Assert.Equal(6, desc.ContentParams.Count);
    }

    [Fact]
    public void ShouldProvideHeaderCollectionParameterDescriptions()
    {
        //Arrange
        var m = GetMethod(nameof(IApiContract.HeaderMethod));

        //Act
        var desc = RequestParametersDescriptions.FromMethod(m, null);

        //Assert
        Assert.NotNull(desc.HeaderCollectionParams);
        Assert.Contains(desc.HeaderCollectionParams, p => p is { Position: 1 });
    }

    [Fact]
    public void ShouldFailWhenUnsupportedParameterAttributeMet()
    {
        //Arrange
        var m = GetMethod(nameof(IApiContract.MethodWithUnsupportedParameterAttr));

        //Act & Assert

        Assert.NotNull(m);
        var e = Assert.Throws<InvalidApiContractException>(() => RequestParametersDescriptions.FromMethod(m, null));
        Assert.Contains("is not supported", e.Message);
    }

    [Fact]
    public void ShouldFailWhenMethodWithoutApiParameterAttribute()
    {
        //Arrange
        var m = GetMethod(nameof(IApiContract.MethodWithoutParameterAttr));

        //Act & Assert

        Assert.NotNull(m);
        var e = Assert.Throws<InvalidApiContractException>(() => RequestParametersDescriptions.FromMethod(m, null));
        Assert.Contains("must be marked with one of inheritors of", e.Message);
    }

    MethodInfo GetMethod(string name)
    {
        return typeof(IApiContract).GetMethod(name) ?? throw new InvalidOperationException("Method not found");
    }
    
    interface IApiContract
    {
        const string PathParamName = "path-param";
        const string QueryParamName = "q-param";
        const string HeaderParamName = "header-param";

        void MethodWithoutParameterAttr(string p);

        void MethodWithUnsupportedParameterAttr([UnsupportedParam] string p);

        void UrlMethod
        (
            [Path(PathParamName)] string path,
            [Query(QueryParamName)] string query
        );

        void HeaderMethod
        (
            [Header(HeaderParamName)] string header,
            [HeaderCollection] IEnumerable<KeyValuePair<string, object>> headers
        );

        void ContentMethod
        (
            [BinContent] byte[] bin,
            [FormContent] object form,
            [JsonContent] object json,
            [XmlContent] object xml,
            [MultipartContent] IMultipartContentParameter multipart,
            [StringContent] string str
        );
    }

    class UnsupportedParamAttribute : ApiParameterAttribute
    {
        
    }
}