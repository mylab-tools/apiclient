using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ApiConfigKeyProviderBehavior
    {
        [Fact]
        public void ShouldProvideConfigKey()
        {
            //Arrange
            
            //Act
            var key = ApiConfigKeyProvider.Provide(typeof(IApiContract));

            //Assert
            Assert.Equal("foo", key);
        }

        [Fact]
        public void ShouldProvideInterfaceNameIfKeyNotSpecified()
        {
            //Arrange

            //Act
            var key = ApiConfigKeyProvider.Provide(typeof(IApiContractWithoutAttrKey));

            //Assert
            Assert.Equal(nameof(IApiContractWithoutAttrKey), key);
        }

        [Fact]
        public void ShouldProvideInterfaceNameIfAttributeNotSpecified()
        {
            //Arrange

            //Act
            var key = ApiConfigKeyProvider.Provide(typeof(IApiContractWithoutAttr));

            //Assert
            Assert.Equal(nameof(IApiContractWithoutAttr), key);
        }

        [Api(Key = "foo")]
        interface IApiContract
        {

        }

        [Api]
        interface IApiContractWithoutAttrKey
        {

        }
        
        interface IApiContractWithoutAttr
        {

        }
    }
}
