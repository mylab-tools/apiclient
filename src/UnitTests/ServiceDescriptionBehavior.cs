using System.ComponentModel.Design;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ServiceDescriptionBehavior
    {
        [Fact]
        public void ShouldFailIfApiAttributeIsAbsent()
        {
            Assert.Throws<ApiContractException>(ServiceDescription<IContractWithoutApiAttr>.Create);
        }

        [Fact]
        public void ShouldDetermineBaseUrl()
        {
            //Act
            var desc = ServiceDescription<IContract>.Create();
            
            //Assert
            Assert.Equal("http://foo", desc.Url);
        }

        interface IContractWithoutApiAttr
        {
            
        }

        [Api("http://foo")]
        interface IContract
        {
            
        }
    }
}