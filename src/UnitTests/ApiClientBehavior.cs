using System;
using System.Net.Http;
using Moq;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ApiClientBehavior
    {
        [Fact]
        public void ShouldFailWhenExpressionIsNotMethodCall()
        {
            //Arrange
            var httpClFactory = new Mock<IHttpClientFactory>().Object;
            var client = ApiClient<IContract>.Create(httpClFactory);
            
            //Act & Assert
            Assert.Throws<NotSupportedException>(() => client.Expression(c => c.Foo() > 2));
        }
        
        [Fact]
        public void ShouldPassWhenExpressionIsMethodCall()
        {
            //Arrange
            var httpClFactory = new Mock<IHttpClientFactory>().Object;
            var client = ApiClient<IContract>.Create(httpClFactory);
            
            //Act & Assert
            client.Expression(c => c.Foo());
        }

        [Api]
        interface IContract
        {
            [Get]
            int Foo();
        }
    }
}