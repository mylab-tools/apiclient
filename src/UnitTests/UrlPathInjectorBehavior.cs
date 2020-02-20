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

        [Theory]
        [InlineData("id", "orders/id/description")]
        [InlineData("", "orders/description")]
        [InlineData(null, "orders/description")]
        [InlineData("a>b", "orders/a%3Eb/description")]
        public void ShouldInjectParameterToPathWithRelative(string paramVal, string expectedUrl)
        {
            //Arrange
            var injector = new UrlPathInjector();
            var originUrl = new Uri("orders/{order-id}/description", UriKind.Relative);

            //Act
            var modifiedUrl = injector.Modify(originUrl, "order-id", paramVal);

            //Assert
            Assert.Equal(expectedUrl, modifiedUrl.OriginalString);
        }
    }
}
