using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class DateTimeSerializationResearch
    {
        [Fact]
        public void ShouldSerializeExpected()
        {
            //Arrange
            var dt = new DateTime(2021, 2, 17, 16,52,26);

            //Act

            //Assert
            Assert.Equal("02/17/2021 16:52:26", dt.ToString(CultureInfo.InvariantCulture));
        }
    }
}
