using NUnit.Framework;
using DotAspectClient;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public partial class WebApiClientFctoryBehavior
    {
        [Test]
        public void ShouldErrorWhenCreateByContractWithoutSpecialAttribute()
        {
            Assert.Throws<WebApiContractException>(() => WebApiClientFactory.CreateProxy<IContractWithoutSpecialAttributes>(""));
        }

        [Test]
        public void ShouldNotErrorWhenCreateByServiceContract()
        {
            WebApiClientFactory.CreateProxy<IService>("http://localhost");
        }

        [Test]
        public void ShouldNotErrorWhenCreateByResourceContract()
        {
            WebApiClientFactory.CreateProxy<IResource>("http://localhost");
        }
    }
}
