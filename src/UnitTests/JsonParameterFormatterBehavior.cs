using System.Collections.Generic;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class JsonParameterFormatterBehavior
    {
        [Theory]
        [MemberData(nameof(GetTestValues))]
        public void ShouldFormatValue(object value, string expectedResult)
        {
            //Arrange
            var f = new JsonParameterFormatter();
            //Act
            var actualResult = f.Format(value);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        public static IEnumerable<object[]> GetTestValues()
        {
            yield return new object[] {null, "null"};
            yield return new object[] {1, "1"};
            yield return new object[] {"foo", "\"foo\""};
            yield return new object[] {new TestJsonObject {Value = "foo"}, "{\"Value\":\"foo\"}"};
        }

        public class TestJsonObject
        {
            public string Value { get; set; }
        }
    }
}
