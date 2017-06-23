using System;
using System.Text;
using NUnit.Framework;
using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public partial class WebApiProxyBehavior
    {
        [Test]
        public void ShouldCastToContract()
        {
            //Arrange
            var proxy = new WebApiProxy<IResourceContract>(new WebApiDescription(), new EmptyRequestProcessor());

            //Act
            var contract = (IResourceContract) proxy.GetTransparentProxy();
        }

        [Test]
        public void ShouldInvokeMethod()
        {
            //Arrange
            var proxy = new WebApiProxy<IResourceContract>(new WebApiDescription(), new EmptyRequestProcessor());

            //Act
            var contract = (IResourceContract)proxy.GetTransparentProxy();
            contract.Foo(1, 2);
        }

        [Test]
        public void ShouldSupportTaskMethodResult()
        {
            //Arrange
            var factory = new WebApiClientFactory<IResourceContract>("http://localhost");
            var proxy = factory.Create();

            //Act
            var task = proxy.GetProcessingTask();
            
            //Assert
            Assert.That(task, Is.Not.Null);
        }

        [Test]
        public void ShouldSupportGenericTaskMethodResult()
        {
            //Arrange
            var factory = new WebApiClientFactory<IResourceContract>("http://localhost");
            var proxy = factory.Create();

            //Act
            var task = proxy.GetGenericProcessingTask();

            //Assert
            Assert.That(task, Is.Not.Null);
        }

        [Test]
        public void ShouldSupportBinaryMethodResult()
        {
            //Arrange
            string payload = Guid.NewGuid().ToString();

            var reqProc = new StringContentRequestProcessor(payload);
            var factory = new WebApiClientFactory<IResourceContract>("http://localhost", reqProc);
            var proxy = factory.Create();

            //Act
            var strResp = proxy.GetString();

            //Assert
            Assert.That(strResp, Is.EqualTo(payload));
        }

        [Test]
        public void ShouldSupportStringMethodResult()
        {
            //Arrange
            string payload = Guid.NewGuid().ToString();

            var reqProc = new StringContentRequestProcessor(payload);
            var factory = new WebApiClientFactory<IResourceContract>("http://localhost", reqProc);
            var proxy = factory.Create();

            //Act
            var strResp = proxy.GetString();

            //Assert
            Assert.That(strResp, Is.EqualTo(payload));
        }

        [Test]
        public void ShouldSupportObjectMethodResultWithXmlDeserialization()
        {
            //Arrange
            string testVal = Guid.NewGuid().ToString("N");
            string payload = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><root>" +
                                 $"<{nameof(PayloadStructuralObject.Value)}>" +
                                    $"{testVal}" +
                                 $"</{nameof(PayloadStructuralObject.Value)}>" +
                             "</root>";

            var reqProc = new StringContentRequestProcessor(payload);
            var factory = new WebApiClientFactory<IResourceContract>("http://localhost", reqProc);
            var proxy = factory.Create();

            //Act
            var resp = proxy.GetObject();

            //Assert
            Assert.That(resp.Value, Is.EqualTo(testVal));
        }

        [Test]
        public void ShouldSupportObjectMethodResultWithJsonDeserialization()
        {
            //Arrange
            string testVal = Guid.NewGuid().ToString();
            string payload = "{" +
                                 $"\"{nameof(PayloadStructuralObject.Value)}\" : \"{Uri.UnescapeDataString(testVal)}\"" + 
                             "}";

            var reqProc = new StringContentRequestProcessor(payload);
            var factory = new WebApiClientFactory<IResourceContract>("http://localhost", reqProc);
            var proxy = factory.Create();

            //Act
            var resp = proxy.GetObject();

            //Assert
            Assert.That(resp.Value, Is.EqualTo(testVal));
        }
    }
}
