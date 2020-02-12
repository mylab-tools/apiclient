using System.Collections.Generic;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class XmlParameterFormatterBehavior
    {
        [Theory]
        [MemberData(nameof(GetTestValues))]
        public void ShouldFormatValue(object value, string expectedResult)
        {
            //Arrange
            var f = new XmlParameterFormatter();
            //Act
            var actualResult = f.Format(value);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        public static IEnumerable<object[]> GetTestValues()
        {
            yield return new object[] {null, null};
            yield return new object[] {1, "<int>1</int>" };
            yield return new object[] {"foo", "<string>foo</string>" };
            yield return new object[] 
            {
                new TestJsonObject {Value = "foo"}, 
                "<TestJsonObject><Value>foo</Value></TestJsonObject>"

            };
        }

        public class TestJsonObject
        {
            public string Value { get; set; }
        }
    }
}
