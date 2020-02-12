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
            var pi = mi.GetParameters().First(p => p.Name == "parameterWithoutAttribute");
            
            //Act & Assert
            Assert.Throws<ApiContractException>(() => InputParameterDescription.Create(pi));
        }

        [Fact]
        public void ShouldDetermineParameterFormatter()
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(nameof(IContract.Example1));
            var pi = mi.GetParameters().First(p => p.Name == "rightParam");
            
            //Act
            var d = InputParameterDescription.Create(pi);
            
            //Assert
            Assert.IsType<StringParameterFormatter>(d.Formatter);
            Assert.IsType<BodyParameterInjector>(d.Injector);
        }

        interface IContract
        {
            [Get("/foo")]
            void Example1(
                int parameterWithoutAttribute, 
                [StringBody] int rightParam);
        }
    }
}