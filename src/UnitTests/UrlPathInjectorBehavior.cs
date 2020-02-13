using System;
using System.Collections.Generic;
using System.Text;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class UrlPathInjectorBehavior
    {
        [Theory]
        [InlineData("id", "http://foo.com/orders/id/description")]
        [InlineData("", "http://foo.com/orders/description")]
        [InlineData(null, "http://foo.com/orders/description")]
        [InlineData("a>b", "http://foo.com/orders/a%3Eb/description")]
        public void ShouldInjectParameterToPath(string paramVal, string expectedUrl)
        {
            //Arrange
            var injector = new UrlPathInjector();
            var originUrl = new Uri("http://foo.com/orders/{order-id}/description");

            //Act
            var modifiedUrl = injector.Modify(originUrl, "order-id", paramVal);

            //Assert
            Assert.Equal(expectedUrl, modifiedUrl.AbsoluteUri);
        }
    }

    public class UrlQueryInjectorBehavior
    {
        [Theory]
        [InlineData("id", "http://foo.com/orders?user=010&order-id=id")]
        [InlineData("", "http://foo.com/orders?user=010&order-id=")]
        [InlineData(null, "http://foo.com/orders?user=010&order-id=")]
        [InlineData("a>b", "http://foo.com/orders?user=010&order-id=a%3Eb")]
        public void ShouldInjectParameterToQuery(string paramVal, string expectedUrl)
        {
            //Arrange
            var injector = new UrlQueryInjector();
            var originUrl = new Uri("http://foo.com/orders?user=010");

            //Act
            var modifiedUrl = injector.Modify(originUrl, "order-id", paramVal);

            //Assert
            Assert.Equal(expectedUrl, modifiedUrl.AbsoluteUri);
        }
    }
}
