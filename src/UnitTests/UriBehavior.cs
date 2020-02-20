using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class UriBehavior
    {
        [Fact]
        public void ShouldCreateRelativeUrl()
        {
            //Arrange

            //Act
            var url = new Uri("path", UriKind.RelativeOrAbsolute);

            //Assert
            Assert.False(url.IsAbsoluteUri);
        }
    }
}
