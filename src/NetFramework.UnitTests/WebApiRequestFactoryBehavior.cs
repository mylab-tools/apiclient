using System.CodeDom;
using NUnit.Framework;
using RedCucumber.Wac;

namespace NetFramework.UnitTests
{
    [TestFixture]
    public class WebApiRequestFactoryBehavior
    {
        [Test]
        public void ShouldSpecifyHttpMethod()
        {
            //Arrange
            var method = HttpMethod.Post;
            var md = new WebApiMethodDescription
            {
                HttpMethod = method
            };
            var reqFact = new WebApiRequestFactory(md, "http://localhost");

            //Act
            var msg = reqFact.CreateMessage(new InvokeParameters());                

            //Assert
            Assert.That(msg.Method.Method, Is.EqualTo(method.ToString().ToUpper()));
        }

        [Test]
        public void ShouldSpecifyGetParameters()
        {
            //Arrange
            var p1 = "p1";
            var p2 = "p2";
            var v2 = "v2";
            var v1 = "v1";

            var md = new WebApiMethodDescription
            {
                Parameters = new WebApiParameterDescriptions(
                    new []
                    {
                        new WebApiParameterDescription
                        {
                            Name = p1,
                            MethodParameterName = p1,
                            Type = WebApiParameterType.Get
                        },
                        new WebApiParameterDescription
                        {
                            Name = p2,
                            MethodParameterName = p2,
                            Type = WebApiParameterType.Get
                        }
                    })
            };
            var reqFact = new WebApiRequestFactory(md, "http://localhost");
            var invokeParams = new InvokeParameters(
                new[] {p1, p2}, 
                new object[] {v1, v2});

            var excpetedQuery = $"?{p1}={v1}&{p2}={v2}";
            //Act

            var msg = reqFact.CreateMessage(invokeParams);

            //Assert
            Assert.That(msg.RequestUri.Query, Is.EqualTo(excpetedQuery));
        }

        [Test]
        public void ShouldUnexcapeParameterNamesAndValues()
        {
            //Arrange
            var p1 = "p1 p1";
            var v1 = "v1 v1";

            var expectedP1 = "p1%20p1";
            var expectedV1 = "v1%20v1";

            var md = new WebApiMethodDescription
            {
                Parameters = new WebApiParameterDescriptions(
                    new[]
                    {
                        new WebApiParameterDescription
                        {
                            Name = p1,
                            MethodParameterName = p1,
                            Type = WebApiParameterType.Get
                        },                    })
            };
            var reqFact = new WebApiRequestFactory(md, "http://localhost");
            var invokeParams = new InvokeParameters(
                new[] { p1 },
                new object[] { v1,});

            var excpetedQuery = $"?{expectedP1}={expectedV1}";
            //Act

            var msg = reqFact.CreateMessage(invokeParams);

            //Assert
            Assert.That(msg.RequestUri.Query, Is.EqualTo(excpetedQuery));
        }
    }
}
