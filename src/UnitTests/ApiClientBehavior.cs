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
            var httpClientProvider = new Mock<IHttpClientProvider>().Object;
            var client = ApiClient<IContract>.Create(httpClientProvider);
            
            //Act & Assert
            Assert.Throws<NotSupportedException>(() => client.Request(c => c.Foo() > 2));
        }
        
        [Fact]
        public void ShouldPassWhenExpressionIsMethodCall()
        {
            //Arrange
            var httpClientProvider = new Mock<IHttpClientProvider>().Object;
            var client = ApiClient<IContract>.Create(httpClientProvider);
            
            //Act & Assert
            client.Request(c => c.Foo());
        }

        [Api]
        interface IContract
        {
            [Get]
            int Foo();
        }
    }
}