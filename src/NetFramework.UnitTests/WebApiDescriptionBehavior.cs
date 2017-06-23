using NUnit.Framework;
using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public class WebApiDescriptionBehavior
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

        [WebApiResource]
        private interface IResourceContractWithWrongMarkedMethod
        {
            [ServiceEndpoint(HttpMethod.Get, "")]
            void WithWrongAttribute();
        }

        [WebApiService]
        private interface IServiceContractWithWronMarkedMethod
        {
            [ResourceAction(HttpMethod.Get)]
            void WithWrongAttribute();
        }

        [WebApiResource]
        interface IContractWithForgottenMethod
        {
            void Foo(int i);
        }
    }
}
