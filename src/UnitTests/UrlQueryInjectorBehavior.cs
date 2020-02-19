using System;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
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

        [Theory]
        [InlineData("id", "http://foo.com/orders?order-id=id")]
        [InlineData("", "http://foo.com/orders?order-id=")]
        [InlineData(null, "http://foo.com/orders?order-id=")]
        [InlineData("a>b", "http://foo.com/orders?order-id=a%3Eb")]
        public void ShouldInjectParameterToQueryWithoutAdditionalQueryParams(string paramVal, string expectedUrl)
        {
            //Arrange
            var injector = new UrlQueryInjector();
            var originUrl = new Uri("http://foo.com/orders");

            //Act
            var modifiedUrl = injector.Modify(originUrl, "order-id", paramVal);

            //Assert
            Assert.Equal(expectedUrl, modifiedUrl.AbsoluteUri);
        }

        [Theory]
        [InlineData("id", "orders?user=010&order-id=id")]
        [InlineData("", "orders?user=010&order-id=")]
        [InlineData(null, "orders?user=010&order-id=")]
        [InlineData("a>b", "orders?user=010&order-id=a%3Eb")]
        public void ShouldInjectParameterToQueryWithRelative(string paramVal, string expectedUrl)
        {
            //Arrange
            var injector = new UrlQueryInjector();
            var originUrl = new Uri("orders?user=010", UriKind.Relative);

            //Act
            var modifiedUrl = injector.Modify(originUrl, "order-id", paramVal);

            //Assert
            Assert.Equal(expectedUrl, modifiedUrl.OriginalString);
        }
    }
}