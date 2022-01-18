using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class XmlHttpContentFactoryBehavior
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlHttpContentFactoryBehavior"/>
        /// </summary>
        public XmlHttpContentFactoryBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetSerializationCases))]
        public async Task ShouldCreateRightHttpContent(object source, string expected)
        {
            //Arrange
            var contentFactory = new XmlHttpContentFactory();

            //Act
            var content = contentFactory.Create(source, null);
            var contentStr = await content.ReadAsStringAsync();

            _output.WriteLine("CONTENT:");
            _output.WriteLine(contentStr);

            //Assert
            Assert.Equal(expected, contentStr);
            Assert.Equal("application/xml", content.Headers.ContentType.MediaType);
            Assert.Equal(expected.Length, content.Headers.ContentLength.GetValueOrDefault(0));
        }

        public static IEnumerable<object[]> GetSerializationCases()
        {
            yield return new object[] { new TestModel { TestValue = "foo" }, "<TestModel><TestValue>foo</TestValue></TestModel>" };
            yield return new object[] { null, "" };
        }

        public class TestModel
        {
            public string TestValue { get; set; }
        }
    }
}