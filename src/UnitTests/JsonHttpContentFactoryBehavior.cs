using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class JsonHttpContentFactoryBehavior
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonHttpContentFactoryBehavior"/>
        /// </summary>
        public JsonHttpContentFactoryBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetSerializationCases))]
        public async Task ShouldCreateRightHttpContent(object source, string expected)
        {
            //Arrange
            var contentFactory = new JsonHttpContentFactory();

            //Act
            var content = contentFactory.Create(source);
            var contentStr = await content.ReadAsStringAsync();

            _output.WriteLine("HEADERS:");

            foreach (var header in content.Headers)
            {
                _output.WriteLine($"\t{header.Key} = {string.Join(',', header.Value)}");
            }

            _output.WriteLine("CONTENT:");
            _output.WriteLine("\t" + contentStr);

            //Assert
            Assert.Equal(expected, contentStr);
            Assert.Equal("application/json", content.Headers.ContentType.MediaType);
            Assert.Equal(expected.Length, content.Headers.ContentLength.GetValueOrDefault(0));
        }

        public static IEnumerable<object[]> GetSerializationCases()
        {
            yield return new object[] {new {Foo = "foo"}, "{\"Foo\":\"foo\"}"};
            yield return new object[] { null, "null"};
        }
    }
}
