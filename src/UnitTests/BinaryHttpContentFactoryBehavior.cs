using System.Text;
using System.Threading.Tasks;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class BinaryHttpContentFactoryBehavior
    {
        [Fact]
        public async Task ShouldCreateHttpContent()
        {
            //Arrange
            var binData = Encoding.UTF8.GetBytes("foo");
            var factory = new BinaryHttpContentFactory();


            //Act
            var content = factory.Create(binData);
            var strRes = await content.ReadAsStringAsync();

            //Assert
            Assert.NotNull(content);
            Assert.NotNull(content.Headers);
            Assert.Equal(binData.Length, content.Headers.ContentLength);
            Assert.NotNull(content.Headers.ContentType);
            Assert.Equal("application/octet-stream", content.Headers.ContentType.MediaType);
            Assert.Equal("foo", strRes);
        }
    }
}
