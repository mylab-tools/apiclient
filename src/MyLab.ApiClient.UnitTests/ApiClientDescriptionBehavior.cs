using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace MyLab.ApiClient.UnitTests
{
    public class ApiClientDescriptionBehavior
    {
        [Fact]
        public void ShouldGetContractDescription()
        {
            //Arrange
            var contract = typeof(ITestApiContract);

            //Act
            var desc = ApiClientDescription.Get(contract);

            //Assert
            Assert.Equal("/foo", desc.RelPath);
        }

        [Fact]
        public void ShouldGetMethodsDescription()
        {
            //Arrange
            var contract = typeof(ITestApiContract);
            var postMethodToken = contract.GetMethod(nameof(ITestApiContract.Post)).MetadataToken;

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
            var contract = typeof(ITestApiContract);
            var postMethodToken = contract.GetMethod(nameof(ITestApiContract.Post)).MetadataToken;

            //Act
            var desc = ApiClientDescription.Get(contract);

            //Assert
            var m = desc.GetMethod(postMethodToken);
            var p = m.Params.FirstOrDefault(param => param.Name == "baz");

            Assert.NotNull(p);
            Assert.Equal(ApiParamPlace.Query, p.Place);
        }

        [Theory]
        [InlineData(typeof(ISimpleInterface))]
        [InlineData(typeof(IMethodWithoutAttribute))]
        [InlineData(typeof(IParameterWithoutAttribute))]
        [InlineData(typeof(ISyncMethod))]
        public void ShouldRejectWhen(Type contractType)
        {
            //Act
            Assert.Throws<ApiDescriptionException>(() => ApiClientDescription.Get(contractType));
        }

        [Api("/foo")]
        interface IParameterWithoutAttribute
        {
            [ApiPost(RelPath = "/bar")]
            Task BadMethod(string p);
        }

        [Api("/foo")]
        interface IMethodWithoutAttribute
        {
            Task BadMethod();
        }

        [Api("/foo")]
        interface ISyncMethod
        {
            void BadMethod();
        }

        interface ISimpleInterface
        {
            
        }
    }
}
