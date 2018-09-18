using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MyLab.ApiClient.UnitTests
{
    public class ClientProxyBehavior
    {
        [Fact]
        public async Task ShouldInvokeStrategy()
        {
            //Arrange
            var strategy = new FakeClientProxyStrategy();
            var apiDesc = ApiClientDescription.Get(typeof(ITestApiContract));

            var proxy = ClientProxy<ITestApiContract>.CreateProxy(apiDesc, strategy);

            //Act
            await proxy.Post("foo");

            //Assert
            Assert.NotNull(strategy.GotDescription);
            Assert.Equal(apiDesc, strategy.GotDescription);
            Assert.NotNull(strategy.Args);
            Assert.Single(strategy.Args);
            Assert.Equal("foo", strategy.Args[0]);
        }

        class FakeClientProxyStrategy : IClientProxyStrategy
        {
            public ApiClientDescription GotDescription { get; private set; }

            public object[] Args { get; private set; }

            public object Invoke(MethodInfo method, ApiClientDescription description, object[] args)
            {
                return Task.Run(() =>
                {
                    GotDescription = description;
                    Args = args;
                });
            }
        }
    }
}
