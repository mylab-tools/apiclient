using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace UnitTests
{
    public class XmlMediaTypeFormatterBehavior
    {
        [Fact]
        public async Task ShouldSerializeWithoutDefaultNamespace()
        {
            //Arrange
            var testData = new TestModel {TestValue = "foo"};
            var content = new ObjectContent<TestModel>(testData, new XmlMediaTypeFormatter
            {
                UseXmlSerializer = true
            });
            var ser = new XmlSerializer(typeof(TestModel));

            //Act
            var str = await content.ReadAsStringAsync();
            using var txtReader= new StringReader(str);
            using var xmlRdr = XmlReader.Create(txtReader);
            var newObj = (TestModel)ser.Deserialize(xmlRdr);

            //Assert
            Assert.NotNull(newObj);
            Assert.Equal("foo", newObj.TestValue);
        }

        public class TestModel
        {
            public string TestValue { get; set; }
        }
    }
}
