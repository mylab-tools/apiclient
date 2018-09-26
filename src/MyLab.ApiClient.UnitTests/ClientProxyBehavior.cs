using System.Net.Http;
using System.Reflection;
using System.Threading;
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
            

            public WebApiCall GetCall(MethodInfo method, ApiClientDescription description, object[] args)
            {
                GotDescription = description;
                Args = args;

                return new WebApiCall(null, new FakeHttpRequestInvoker());
            }

            public WebApiCall<TResult> GetCall<TResult>(MethodInfo method, ApiClientDescription description, object[] args)
            {
                throw new System.NotImplementedException();
            }
        }

        class FakeHttpRequestInvoker : IHttpRequestInvoker
        {
            public async Task<HttpResponseMessage> Send(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new HttpResponseMessage());
            }
        }
    }
}
