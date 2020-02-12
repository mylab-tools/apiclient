using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class MethodDescriptionBehavior
    {   
        [Fact]
        public void ShouldFailIfMethodAttributeIsAbsent()
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(nameof(IContract.WithoutMethodAttribute));
            
            //Act & Assert
            Assert.Throws<ApiContractException>(() => MethodDescription.Create(mi));
        }

        [Fact]
        public void ShouldDetermineMethodParameters()
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(nameof(IContract.WithAttribute));
            
            //Act
            var d = MethodDescription.Create(mi);
            
            //Assert
            Assert.Equal("GET", d.HttpMethod);
            Assert.Equal("/foo", d.Url);
        }

        interface IContract
        {
            void WithoutMethodAttribute();
            
            [Get("/foo")]
            void WithAttribute();
        }
    }
}