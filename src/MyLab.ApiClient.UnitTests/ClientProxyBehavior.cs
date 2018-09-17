using Xunit;

namespace MyLab.ApiClient.UnitTests
{
    public class ClientProxyBehavior
    {
        [Fact]
        public void ShouldInvokeStrategy()
        {
            //Arrange
            var strategy = new FakeClientProxyStrategy();

            var apiDesc = ApiClientDescription.Get(typeof(ITestApiContract));
            var postMethodToken = typeof(ITestApiContract).GetMethod(nameof(ITestApiContract.Post)).MetadataToken;
            var postMethodDesc = apiDesc.GetMethod(postMethodToken);

            var proxy = ClientProxy<ITestApiContract>.CreateProxy(apiDesc, strategy);

            //Act
            proxy.Post("foo");

            //Assert
            Assert.NotNull(strategy.GotDescription);
            Assert.Equal(postMethodDesc, strategy.GotDescription);
            Assert.NotNull(strategy.Args);
            Assert.Single(strategy.Args);
            Assert.Equal("foo", strategy.Args[0]);
        }

        class FakeClientProxyStrategy : IClientProxyStrategy
        {
            public MethodDescription GotDescription { get; private set; }

            public object[] Args { get; private set; }

            public object Invoke(MethodDescription methodDescription, object[] args)
            {
                GotDescription = methodDescription;
                Args = args;

                return null;
            }
        }
    }
}
