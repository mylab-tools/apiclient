using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotAspectClient;
using HttpMethod = DotAspectClient.HttpMethod;

namespace NetFramework.UnitTests
{
    public partial class WebApiProxyBehavior
    {
        private class EmptyRequestProcessor : IWebApiRequestProcessor
        {
            /// <inheritdoc />
            public Task<System.Net.Http.HttpResponseMessage> ProcessRequest(System.Net.Http.HttpRequestMessage message)
            {
                return Task.FromResult<System.Net.Http.HttpResponseMessage>(null);
            }
        }

        [RestApi]
        private interface IResourceContract
        {
            [RestAction(HttpMethod.Get)]
            void Foo(int a, int b);
            [RestAction(HttpMethod.Get)]
            Task GetProcessingTask();
            [RestAction(HttpMethod.Get)]
            Task<System.Net.Http.HttpResponseMessage> GetGenericProcessingTask();
            [RestAction(HttpMethod.Get)]
            byte[] GetBinary();
            [RestAction(HttpMethod.Get)]
            string GetString();
            [RestAction(HttpMethod.Get)]
            PayloadStructuralObject GetObject();
        }

        class BinaryContentRequestProcessor : IWebApiRequestProcessor
        {
            private readonly byte[] _response;

            public BinaryContentRequestProcessor(byte[] response)
            {
                _response = response;
            }
            /// <inheritdoc />
            public Task<System.Net.Http.HttpResponseMessage> ProcessRequest(HttpRequestMessage message)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(_response)
                };

                return Task.FromResult(msg);
            }
        }

        class StringContentRequestProcessor : IWebApiRequestProcessor
        {
            private readonly string _response;

            public StringContentRequestProcessor(string response)
            {
                _response = response;
            }
            /// <inheritdoc />
            public Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(_response)
                };

                return Task.FromResult(msg);
            }
        }

        public class PayloadStructuralObject
        {
            public string Value { get; set; }
        }
    }
}