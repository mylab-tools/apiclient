using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
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

        [Fact]
        public void ShouldPassCancellationTokenParameterWithoutAttribute()
        {
            //Arrange
            var contract = typeof(IContractWithCancellation);

            //Act & Assert
            ApiClientDescription.Get(contract);
        }
        
        
        [Fact]
        public void ShouldRejectContentParametersConflicts()
        {
            //Arrange
            var contract = typeof(IContractWithParameterConflicts);
            var m = contract.GetMethod(nameof(IContractWithParameterConflicts.MultiBody));

            //Act & Assert
            Assert.Throws<ApiDescriptionException>(() => MethodDescription.Get(m));
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

        [Fact]
        public void ShouldRejectNotBinaryParameterWithBinBodyAttr()
        {
            //Arrange
            var contract = typeof(IContractWithBadBinaryParameter);
            var m = contract.GetMethod(nameof(IContractWithBadBinaryParameter.BadMethod));

            //Act & Assert
            Assert.Throws<ApiDescriptionException>(() => MethodDescription.Get(m));
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
            Task BadMethod();
        }

        interface ISimpleInterface
        {
            
        }

        [Api("/foo")]
        interface IContractWithCancellation
        {
            [ApiPost(RelPath = "/bar")]
            Task PostWithCancel(
                [QueryParam(Name = "baz")] string parameter,
                CancellationToken cancellationToken
            );
        }

        [Api("/foo")]
        interface IContractWithParameterConflicts
        {
            [ApiPost]
            Task MultiBody(
                [TextBody] string p1,
                [TextBody] string p2
                );
        }

        [Api("/foo")]
        interface IContractWithBadBinaryParameter
        {
            [ApiPost]
            Task BadMethod([BinaryBody] string p);
        }
    }
}
