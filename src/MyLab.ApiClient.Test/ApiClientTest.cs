using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Contains base functional for test with API client
    /// </summary>
    public class ApiClientTest<TStartup, TService> : IDisposable
        where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> _appFactory;
        private readonly ApiClient<TService> _apiClient;

        protected ITestOutputHelper Output { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClientTest{TStartup,TService}"/>
        /// </summary>
        public ApiClientTest(ITestOutputHelper output)
        {
            Output = output;
            _appFactory = new WebApplicationFactory<TStartup>();
            _apiClient = new ApiClient<TService>(new DelegateHttpClientProvider(() => _appFactory.CreateClient())); 
        }

        public void Dispose()
        {
            _appFactory?.Dispose();
        }

        protected async Task<CallDetails<string>> TestCall(Expression<Func<TService, Task>> invoker)
        {
            var details = await _apiClient.Call(invoker).GetDetailed();

            Log(details);

            return details;
        }

        protected async Task<CallDetails<TRes>> TestCall<TRes>(Expression<Func<TService, Task<TRes>>> invoker)
        {
            var details = await _apiClient.Call(invoker).GetDetailed();

            Log(details);

            return details;
        }

        void Log<TRes>(CallDetails<TRes> call)
        {
            Output.WriteLine("");
            Output.WriteLine("===== REQUEST BEGIN =====");
            Output.WriteLine("");
            Output.WriteLine(call.RequestDump);
            Output.WriteLine("===== REQUEST END =====");
            Output.WriteLine("");
            Output.WriteLine("===== RESPONSE BEGIN =====");
            Output.WriteLine("");
            Output.WriteLine(call.ResponseDump);
            Output.WriteLine("===== RESPONSE END =====");
        }
    }
}
