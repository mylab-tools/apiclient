using System.Linq;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class InputParameterDescriptionBehavior
    {   
        [Fact]
        public void ShouldFailIfParameterAttributeIsAbsent()
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(nameof(IContract.Example1));
            
            //Act & Assert
            Assert.Throws<ApiContractException>(() => RequestParametersDescriptions.Create(mi));
        }

        [Fact]
        public void ShouldDetermineParameterModifier()
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(nameof(IContract.Example2));
            
            //Act
            var d = RequestParametersDescriptions.Create(mi).UrlParams.Single();
            
            //Assert;
            Assert.IsType<UrlPathInjector>(d.Modifier);
        }

        [Theory]
        [InlineData(nameof(IContract.Example2), "rp")]
        [InlineData(nameof(IContract.Example3), "rightParam")]
        public void ShouldOverrideParameterNameByAttribute(string methodName, string expectedParamName)
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(methodName);

            //Act
            var d = RequestParametersDescriptions.Create(mi).UrlParams.Single();

            //Assert;
            Assert.Equal(expectedParamName, d.Name);
        }

        interface IContract
        {
            [Get("/foo")]
            void Example1(int parameterWithoutAttribute);

            [Get("/bar")]
            void Example2([Path("rp")] int rightParam);

            [Get("/baz")]
            void Example3([Path] int rightParam);
        }
    }
}