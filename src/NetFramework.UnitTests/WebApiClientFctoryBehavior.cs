using System;
using System.Collections.Generic;
using NUnit.Framework;
using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public partial class WebApiClientFctoryBehavior
    {
        [Test]
        public void ShouldErrorWhenCreateByContractWithoutSpecialAttribute()
        {
            //Arrange
            var factory = new WebApiClientFactory<IContractWithoutSpecialAttributes>("");
            //Act
            Assert.Throws<WebApiContractException>(() => factory.Create());
        }

        [Test]
        public void ShouldNotErrorWhenCreateByServiceContract()
        {
            //Arrange
            var factory = new WebApiClientFactory<IService>("http://localhost");
            //Act
            factory.Create();
        }

        [Test]
        public void ShouldNotErrorWhenCreateByResourceContract()
        {
            //Arrange
            var factory = new WebApiClientFactory<IResource>("http://localhost");
            //Act
            factory.Create();
        }
    }
}
