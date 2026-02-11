using JetBrains.Annotations;
using MyLab.ApiClient.Contracts.Attributes.ForMethod;
using MyLab.ApiClient.Contracts.Descriptions;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using MyLab.ApiClient.Contracts;
using MyLab.ApiClient.Contracts.Attributes.ForParameters;
using Xunit;

namespace MyLab.ApiClient.Tests.Contracts.Descriptions
{
    [TestSubject(typeof(EndpointDescription))]
    public class EndpointDescriptionBehavior
    {
        [Fact]
        public void ShouldProvideMethodDescription()
        {
            //Arrange
            var m = GetMethod(nameof(IApiContract.Method));

            //Act
            var desc = EndpointDescription.FromMethod(m, null);

            //Assert
            Assert.NotNull(desc);
            Assert.Equal(IApiContract.PostUrl, desc.Url);
            Assert.Equal(HttpMethod.Post, desc.HttpMethod);
            Assert.NotNull(desc.ExpectedStatusCodes);
            Assert.Contains(HttpStatusCode.BadRequest, desc.ExpectedStatusCodes);
            Assert.NotNull(desc.Parameters);
            Assert.Single(desc.Parameters);
            Assert.Contains(desc.Parameters, p => p is UrlParameterDescription { Name: "urlParam", Position:0 });
        }
        
        [Fact]
        public void ShouldFailWhenMethodDoesNotHaveApuMethodAttribute()
        {
            //Arrange
            var m = GetMethod(nameof(IApiContract.MethodWithoutApiMethodAttribute));

            //Act && Assert
            var e = Assert.Throws<InvalidApiContractException>(() => EndpointDescription.FromMethod(m, null));
            Assert.Contains("must be marked with one of", e.Message);
        }

        MethodInfo GetMethod(string name)
        {
            return typeof(IApiContract).GetMethod(name) ?? throw new InvalidOperationException("Method not found");
        }

        interface IApiContract
        {
            public const string PostUrl = "/path";
            
            void MethodWithoutApiMethodAttribute();

            [Post(PostUrl)]
            [ExpectedResponse(HttpStatusCode.BadRequest)]
            void Method([Path] string urlParam);
        }
    }
}
