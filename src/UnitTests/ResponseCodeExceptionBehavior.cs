using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ResponseCodeExceptionBehavior
    {
        [Theory]
        [InlineData(HttpStatusCode.BadRequest, "Some message", "400(BadRequest). Some message.")]
        [InlineData(HttpStatusCode.BadRequest, null, "400(BadRequest). [no message].")]
        public void ShouldCreateRightDescription(HttpStatusCode statusCode, string msg, string expected)
        {
            //Arrange
            

            //Act
            var d = ResponseCodeException.CreateDescription(statusCode, msg);

            //Assert
            Assert.Equal(expected, d);
        }
    }
}
