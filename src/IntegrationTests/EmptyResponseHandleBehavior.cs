using System.Threading.Tasks;
using MyLab.ApiClient;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class EmptyResponseHandleBehavior : MyLab.ApiClient.Test.ApiClientTest<Startup, EmptyResponseHandleBehavior.ITestServer>
    {
        public EmptyResponseHandleBehavior(ITestOutputHelper output)
            :base(output)
        {
        }

        [Fact]
        public async Task ShouldReturnNullIfStringResponseIsEmpty()
        {
            //Arrange

            //Act 
            var resDet = await TestCall(s => s.GetNullString());
            
            //Assert
            Assert.Null(resDet.ResponseContent);
        }

        [Fact]
        public async Task ShouldReturnNullIfValueResponseIsEmpty()
        {
            //Arrange
            
            //Act 
            var resDet = await TestCall(s => s.GetNullValue());
            
            //Assert
            Assert.Equal(default, resDet.ResponseContent);
        }

        [Fact]
        public async Task ShouldReturnNullIfArrayResponseIsEmpty()
        {
            //Arrange
            
            //Act 
            var resDet = await TestCall(s => s.GetNullArray());

            //Assert
            Assert.Null(resDet.ResponseContent);
        }

        [Api("echo/empty")]
        public interface ITestServer
        {
            [Get]
            Task<string> GetNullString();

            [Get]
            Task<int> GetNullValue();

            [Get]
            Task<byte[]> GetNullArray();
        }
    }
}
