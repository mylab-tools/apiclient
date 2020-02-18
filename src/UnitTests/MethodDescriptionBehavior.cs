using System.Linq;
using System.Net;
using System.Net.Http;
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
            Assert.Equal(HttpMethod.Get, d.HttpMethod);
            Assert.Equal("/foo", d.Url);
        }

        [Fact]
        public void ShouldDetermineExpectedHttpCodes()
        {
            //Arrange
            var mi = typeof(IContract).GetMethod(nameof(IContract.WithAttribute));

            //Act
            var d = MethodDescription.Create(mi);

            //Assert
            Assert.Equal(2, d.ExpectedStatusCodes.Count);
            Assert.Contains(HttpStatusCode.OK, d.ExpectedStatusCodes);
            Assert.Contains(HttpStatusCode.BadRequest, d.ExpectedStatusCodes);
        }

        interface IContract
        {
            void WithoutMethodAttribute();
            
            [ExpectedCode(HttpStatusCode.OK)]
            [ExpectedCode(HttpStatusCode.BadRequest)]
            [Get("/foo")]
            void WithAttribute();
        }
    }
}