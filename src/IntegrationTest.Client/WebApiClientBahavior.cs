using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RedCucumber.Wac;
using HttpMethod = RedCucumber.Wac.HttpMethod;

namespace IntegrationTest.Client
{
    [TestFixture]
    public class WebApiClientBahavior
    {
        private WebApiClientFactory<IStringResource> _stringResourceClientFactory;
        private WebApiClientFactory<IEmptyResource> _emptyResourceClientFactory;
        private WebApiClientFactory<IBinaryResource> _binResourceClientFactory;
        private WebApiClientFactory<IStringService> _stringServiceClientFactory;

        [OneTimeSetUp]
        public void BeforeTestSuite()
        {
            _stringResourceClientFactory = new WebApiClientFactory<IStringResource>(
                "http://localhost.loc/IntegrationTest.Server/rest/StringResource");
            _emptyResourceClientFactory = new WebApiClientFactory<IEmptyResource>(
                "http://localhost.loc/IntegrationTest.Server/rest/EmptyResource");
            _binResourceClientFactory= new WebApiClientFactory<IBinaryResource>(
                "http://localhost.loc/IntegrationTest.Server/rest/BinaryResource");
            _stringServiceClientFactory=new WebApiClientFactory<IStringService>(
                "http://localhost.loc/IntegrationTest.Server/api/StringService");
        }

        [OneTimeTearDown]
        public void AfterTestSuite()
        {

        }

        [Test]
        public void ShouldSupportMethodsWithVoidReturn()
        {
            //Arrange
            var client = _emptyResourceClientFactory.Create();

            //Act
            client.GetRequest();
        }

        [Test]
        public void ShouldSupportTaskMethodResult()
        {
            //Arrange
            var client = _emptyResourceClientFactory.Create();

            //Act
            var task = client.GetRequestTask();
            task.Wait();
        }

        [Test]
        public void ShouldSupportTaskMethodResultWithMessage()
        {
            //Arrange
            var client = _stringResourceClientFactory.Create();
            string testString = Guid.NewGuid().ToString();

            //Act
            var task = client.GetStringTaskWithMessage(testString);
            task.Wait();
            var responseString = task.Result.Content.ReadAsStringAsync().Result.Trim('\"');

            //Assert
            Assert.That(responseString, Is.EqualTo(testString));
        }

        [Test]
        public void ShouldReceiveStringResult()
        {
            //Arrange
            var client = _stringResourceClientFactory.Create();
            string testString = Guid.NewGuid().ToString();

            //Act
            var responseString = client.GetString(testString);

            //Assert
            Assert.That(responseString, Is.EqualTo(testString));
        }

        [Test]
        public void ShouldReceiveBinaryPayload()
        {
            //Arrange
            var client = _binResourceClientFactory.Create();
            string testString = Guid.NewGuid().ToString();
            var testBin = Encoding.UTF8.GetBytes(testString);

            //Act
            var responseBin = client.GetBin(new WebApiFile
            {
                Content = testBin,
                Name = "Filename"
            });
            var responseStr = Encoding.UTF8.GetString(responseBin);

            //Assert
            Assert.That(responseStr, Is.EqualTo(testString));
        }

        [Test]
        public void ShouldUseGetParamsWhenPostMethod()
        {
            //Arrange
            var client = _stringServiceClientFactory.Create();

            //Act
            var res = client.ConcatStrings("p1", "p2", "p3");

            //Assert
            Assert.That(res, Is.EqualTo("p1p2p3"));
        }

        [Test]
        public void ShouldCorrectUseJsonSerialization()
        {
            //Arrange
            var client = _stringServiceClientFactory.Create();
            var sendObj = new DataObject
            {
                Value = Guid.NewGuid().ToString()
            };

            //Act
            var gotObj = client.GetJsonBack(sendObj);

            //Assert
            Assert.That(gotObj, Is.Not.Null);
            Assert.That(gotObj.Value, Is.EqualTo(sendObj.Value));
        }

        [Test]
        public void ShouldCorrectUseXmlSerialization()
        {
            //Arrange
            var client = _stringServiceClientFactory.Create();
            var sendObj = new DataObject
            {
                Value = Guid.NewGuid().ToString()
            };

            //Act
            var gotObj = client.GetXmlBack(sendObj);

            //Assert
            Assert.That(gotObj, Is.Not.Null);
            Assert.That(gotObj.Value, Is.EqualTo(sendObj.Value));
        }
    }

    [WebApiResource]
    interface IBinaryResource
    {
        [ResourceAction(HttpMethod.Post)]
        byte[] GetBin(WebApiFile file);
    }

    [WebApiResource]
    interface IEmptyResource
    {
        [ResourceAction(HttpMethod.Get)]
        void GetRequest();

        [ResourceAction(HttpMethod.Get)]
        Task GetRequestTask();
    }

    [WebApiResource]
    interface IStringResource
    {
        [ResourceAction(HttpMethod.Get)]
        Task<HttpResponseMessage> GetStringTaskWithMessage(string str);

        [ResourceAction(HttpMethod.Get)]
        string GetString(string str);
    }

    [WebApiService]
    interface IStringService
    {
        [ServiceEndpoint(HttpMethod.Post)]
        [ContentType(ContentType.Text)]
        string ConcatStrings(string main, [GetParam]string add1, [GetParam]string add2);

        [ServiceEndpoint(HttpMethod.Post)]
        [ContentType(ContentType.Json)]
        DataObject GetJsonBack(DataObject obj);

        [ServiceEndpoint(HttpMethod.Post)]
        [ContentType(ContentType.Xml)]
        DataObject GetXmlBack(DataObject obj);
    }

    public class DataObject
    {
        public string Value { get; set; }
    }
}
