using System;
using System.Net.Http;
using Xunit;

namespace MyLab.ApiClient.UnitTests
{
    public class ApiClientDescriptionBehavior
    {
        [Fact]
        public void ShouldGetContractDescription()
        {
            //Arrange
            var contract = typeof(IApiContract);

            //Act
            var desc = ApiClientDescription.Get(contract);

            //Assert
            Assert.Equal("/foo", desc.RelAddress);
        }

        [Fact]
        public void ShouldGetMethodsDescription()
        {
            //Arrange
            var contract = typeof(IApiContract);
            var postMethodToken = contract.GetMethod(nameof(IApiContract.Post)).MetadataToken;

            //Act
            var desc = ApiClientDescription.Get(contract);

            //Assert
            var m = desc.GetMethod(postMethodToken);

            Assert.Equal("/bar", m.RelPath);
            Assert.Equal(HttpMethod.Post, m.HttpMethod);
        }

        [Fact]
        public void ShouldGetMethodParametersDescription()
        {
            //Arrange
            var contract = typeof(IApiContract);
            var postMethodToken = contract.GetMethod(nameof(IApiContract.Post)).MetadataToken;

            //Act
            var desc = ApiClientDescription.Get(contract);

            //Assert
            var m = desc.GetMethod(postMethodToken);
            var p = m.GetParameter("parameter");

            Assert.Equal("baz", p.Name);
            Assert.Equal(ApiParamPlace.Query, p.Place);
        }

        [Theory]
        [InlineData(typeof(ISimpleInterface))]
        [InlineData(typeof(IContractWithBadMethod))]
        [InlineData(typeof(IContractWithBadParameter))]
        public void ShouldRejectWhenAttributeIsAbsent(Type contractType)
        {
            //Act
            Assert.Throws<ApiDescriptionException>(() => ApiClientDescription.Get(contractType));
        }

        [Api("/foo")]
        interface IContractWithBadParameter
        {
            [ApiPost(RelPath = "/bar")]
            void BadMethod(string p);
        }

        [Api("/foo")]
        interface IContractWithBadMethod
        {
            void BadMethod();
        }

        interface ISimpleInterface
        {
            
        }

        [Api("/foo")]
        interface IApiContract
        {
            [ApiPost(RelPath = "/bar")]
            void Post(
                [ApiParam(ApiParamPlace.Query, Name = "baz")] string parameter
                );
        }
    }
}
