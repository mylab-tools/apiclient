using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient;
using Xunit;

namespace UnitTests
{
    public class ApiProxyBehavior
    {
        [Fact]
        public void ShouldBeProvidedInSingleton()
        {
            //Arrange

            ServiceProvider services = new ServiceCollection()
                .AddApiClients(
                    r => r.RegisterContract<ITestApi>(),
                    new ApiClientsOptions
                    {
                        List = 
                        {
                            {"test-key", new ApiConnectionOptions()}
                        }
                    })
                .AddSingleton<SingletonService>()
                .BuildServiceProvider();

            //Act
            var ss = services.GetService<SingletonService>();

            //Assert
            Assert.NotNull(ss);
            Assert.NotNull(ss.TestApi);
        }

        [Api("bla", Key = "test-key")]
        interface ITestApi
        {
            [Get]
            Task Foo();
        }

        class SingletonService
        {
            public ITestApi TestApi { get; }

            public SingletonService(ITestApi testApi)
            {
                TestApi = testApi;
            }
        }
    }
}
