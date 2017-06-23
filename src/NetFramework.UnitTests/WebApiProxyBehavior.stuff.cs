using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RedCucumber.Wac;
using HttpMethod = RedCucumber.Wac.HttpMethod;

namespace NetFramework.UnitTests
{
    public partial class WebApiProxyBehavior
    {
        private class EmptyRequestProcessor : IWebApiRequestProcessor
        {
            /// <inheritdoc />
            public Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message)
            {
                return Task.FromResult<HttpResponseMessage>(null);
            }
        }

        [WebApiResource]
        private interface IResourceContract
        {
            [ResourceAction(HttpMethod.Get)]
            void Foo(int a, int b);
            [ResourceAction(HttpMethod.Get)]
            Task GetProcessingTask();
            [ResourceAction(HttpMethod.Get)]
            Task<HttpResponseMessage> GetGenericProcessingTask();
            [ResourceAction(HttpMethod.Get)]
            byte[] GetBinary();
            [ResourceAction(HttpMethod.Get)]
            string GetString();
            [ResourceAction(HttpMethod.Get)]
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
            public Task<HttpResponseMessage> ProcessRequest(HttpRequestMessage message)
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