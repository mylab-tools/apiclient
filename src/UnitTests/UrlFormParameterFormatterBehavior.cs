using System;
using System.Collections.Generic;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class UrlFormParameterFormatterBehavior
    {
        [Theory]
        [MemberData(nameof(GetTestValues))]
        public void ShouldFormatValue(object value, string expectedResult)
        {
            //Arrange
            var f = new UrlFormParameterFormatter();
            //Act
            var actualResult = f.Format(value);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [MemberData(nameof(GetUnsupportedValues))]
        public void ShouldNotSupportPrimitiveTypes(object value)
        {
            //Arrange
            var f = new UrlFormParameterFormatter();

            //Act & Assert
            Assert.Throws<NotSupportedException>(() => f.Format(value));
        }

        public static IEnumerable<object[]> GetTestValues()
        {
            yield return new object[] {null, ""};
            yield return new object[] {new TestJsonObject
                {
                    Value1 = "foo",
                    Value2 = new TestJsonObject
                    {
                        Value1 = "bar"
                    }
                },
                "Value1=foo&Value2=%7b%22Value1%22%3a%22bar%22%2c%22Value2%22%3anull%7d"};
        }

        public static IEnumerable<object[]> GetUnsupportedValues()
        {
            yield return new object[] {1};
            yield return new object[] {"foo"};
            yield return new object[] {DateTime.Now};
            yield return new object[] {Guid.Empty};
            yield return new object[] {TimeSpan.MaxValue};
        }

        public class TestJsonObject
        {
            public string Value1 { get; set; }
            public TestJsonObject Value2 { get; set; }
        }
    }
}
