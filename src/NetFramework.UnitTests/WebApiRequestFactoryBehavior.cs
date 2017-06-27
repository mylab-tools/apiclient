using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DotAspectClient;

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

        [Test]
        public void ShouldAddHeadersFromParameters()
        {
            //Arrange
            var p1 = "p1";
            var v1 = "v1";

            var headers = new WepApiMethodHeaders
            {
                new WebApiMethodHeader
                {
                    HeaderName = p1,
                    ParameterName = p1
                }
            };

            var md = new WebApiMethodDescription
            {
                Headers = headers
            };

            var reqFact = new WebApiRequestFactory(md, "http://localhost");
            var invokeParams = new InvokeParameters(
                new[] { p1 },
                new object[] { v1, });
            //Act

            var msg = reqFact.CreateMessage(invokeParams);

            //Assert
            Assert.That(msg.Headers.Contains(p1));
            var headerVal = msg.Headers.First().Value;

            if (headerVal != null)
            {
                var hv = headerVal.ToArray();
                Assert.That(hv.FirstOrDefault(), Is.EqualTo(v1));
            }
            else
            {
                Assert.Fail();
            }
            
        }

        [Test]
        public void ShouldAddPredefinedHeaders()
        {
            //Arrange
            var p1 = "p1";
            var v1 = "v1";

            var md = new WebApiMethodDescription();

            var reqFact = new WebApiRequestFactory(md, "http://localhost")
            {
                Options = new WebApiClientOptions
                {
                    PredefinedHeaders = new []
                    {
                        new RequestHeader
                        {
                            Name = p1,
                            Value = v1
                        }
                    }
                }
            };

            var invokeParams = new InvokeParameters(
                new[] { p1 },
                new object[] { v1, });
            //Act

            var msg = reqFact.CreateMessage(invokeParams);

            //Assert
            Assert.That(msg.Headers.Contains(p1));
            var headerVal = msg.Headers.First().Value;

            if (headerVal != null)
            {
                var hv = headerVal.ToArray();
                Assert.That(hv.FirstOrDefault(), Is.EqualTo(v1));
            }
            else
            {
                Assert.Fail();
            }

        }
    }
}
