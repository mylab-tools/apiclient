using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DotApiClient;

namespace IntegrationTest.Client
{
    [TestFixture]
    public class WebApiClientBahavior
    {
        private WebApiClientFactory<IStringService> _stringServiceClientFactory;
        private WebApiClientFactory _restPointClientFactory;

        [OneTimeSetUp]
        public void BeforeTestSuite()
        {
            _restPointClientFactory = new WebApiClientFactory("http://localhost.loc/IntegrationTest.Server/rest/");
            var stringServiceClientFactory = new WebApiClientFactory("http://localhost.loc/IntegrationTest.Server/api/StringService");
            _stringServiceClientFactory = stringServiceClientFactory.CreateTypedFactory<IStringService>();
        }

        [OneTimeTearDown]
        public void AfterTestSuite()
        {

        }

        [Test]
        public void ShouldSupportMethodsWithVoidReturn()
        {
            //Arrange
            var client = _restPointClientFactory.CreateProxy<IEmptyResource>();

            //Act
            client.GetRequest();
        }

        [Test]
        public void ShouldSupportTaskMethodResult()
        {
            //Arrange
            var client = _restPointClientFactory.CreateProxy<IEmptyResource>();

            //Act
            var task = client.GetRequestTask();
            task.Wait();
        }

        [Test]
        public void ShouldSupportTaskMethodResultWithMessage()
        {
            //Arrange
            var client = _restPointClientFactory.CreateProxy<IStringResource>();
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
            var client = _restPointClientFactory.CreateProxy<IStringResource>();
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
            var client = _restPointClientFactory.CreateProxy<IBinaryResource>();
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
            var client = _stringServiceClientFactory.CreateProxy();

            //Act
            var res = client.ConcatStrings("p1", "p2", "p3");

            //Assert
            Assert.That(res, Is.EqualTo("p1p2p3"));
        }

        [Test]
        public void ShouldCorrectUseJsonSerialization()
        {
            //Arrange
            var client = _stringServiceClientFactory.CreateProxy();
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
            var client = _stringServiceClientFactory.CreateProxy ();
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

        [Test]
        public void ShouldUnderstandArrayResult()
        {
            //Arrange
            var stringArray = new string[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var client = _stringServiceClientFactory.CreateProxy();

            //Act
            var gotArr = client.GetStringArray(stringArray);

            //Assert
            Assert.That(gotArr, Is.Not.Null);
            CollectionAssert.AreEqual(stringArray, gotArr);
        }
    }

    [RestApi(RelPath = "BinaryResource")]
    interface IBinaryResource
    {
        [RestAction(HttpMethod.Post)]
        byte[] GetBin(WebApiFile file);
    }

    [RestApi(RelPath = "EmptyResource")]
    interface IEmptyResource
    {
        [RestAction(HttpMethod.Get)]
        void GetRequest();

        [RestAction(HttpMethod.Get)]
        Task GetRequestTask();
    }

    [RestApi(RelPath = "StringResource")]
    interface IStringResource
    {
        [RestAction(HttpMethod.Get)]
        Task<System.Net.Http.HttpResponseMessage> GetStringTaskWithMessage(string str);

        [RestAction(HttpMethod.Get)]
        string GetString(string str);
    }

    [WebApi]
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

        [ServiceEndpoint(HttpMethod.Post)]
        [ContentType(ContentType.Json)]
        string[] GetStringArray(string[] stringArr);
    }

    public class DataObject
    {
        public string Value { get; set; }
    }
}
