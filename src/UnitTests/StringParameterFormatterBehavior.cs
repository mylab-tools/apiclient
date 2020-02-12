using System.Collections.Generic;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class StringParameterFormatterBehavior
    {
        [Theory]
        [MemberData(nameof(GetTestValues))]
        public void ShouldFormatValue(object value, string expectedResult)
        {
            //Arrange
            var f = new StringParameterFormatter();
            //Act
            var actualResult = f.Format(value);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        public static IEnumerable<object[]> GetTestValues()
        {
            yield return new object[] {null, null};
            yield return new object[] {1, "1"};
            yield return new object[] {"foo", "foo"};
            yield return new object[] {new TestStringObject(), TestStringObject.Value};
        }

        public class TestStringObject
        {
            public const string Value = "bar";

            public override string ToString()
            {
                return Value;
            }
        }
    }
}
