using NUnit.Framework;
using DotApiClient;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public partial class WebApiDescriptionBehavior
    {
        [Test]
        public void ShouldNotAllowToUseResourceMethodAttributesInServiceContract()
        {
            Assert.Throws<WebApiContractException>(() =>
            {
                WebApiDescription.Create(typeof(IServiceContractWithWronMarkedMethod));
            });
        }

        [Test]
        public void ShouldNotAllowToUseServiceMethodAttributesInResourceContract()
        {
            Assert.Throws<WebApiContractException>(() =>
            {
                WebApiDescription.Create(typeof(IResourceContractWithWrongMarkedMethod));
            });
        }

        [Test]
        public void ShouldThrowErrorWhenOneOrMoreMethodsNotMarkedAsWebApiMethod()
        {
            Assert.Throws<WebApiContractException>(() =>
            {
                WebApiDescription.Create(typeof(IContractWithForgottenMethod));
            });
        }
    }
}
