using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides abilities to tune and send request
    /// </summary>
    public class ApiRequest<TRes>
    {
        /// <summary>
        /// Gets request modifiers collection
        /// </summary>
        public List<IRequestMessageModifier> RequestModifiers { get; }
            = new List<IRequestMessageModifier>();

        /// <summary>
        /// Contains expected response http status codes
        /// </summary>
        public List<HttpStatusCode> ExpectedCodes { get; }
            = new List<HttpStatusCode>();

        private readonly string _baseUrl;
        private readonly MethodDescription _methodDescription;
        private readonly HttpClient _httpClient;
        private readonly IReadOnlyList<IParameterApplier> _paramAppliers;

        internal ApiRequest(
            string baseUrl,
            MethodDescription methodDescription, 
            IEnumerable<IParameterApplier> paramAppliers,
            HttpClient httpClient)
        {
            if (paramAppliers == null) throw new ArgumentNullException(nameof(paramAppliers));
            _baseUrl = baseUrl;
            _methodDescription = methodDescription ?? throw new ArgumentNullException(nameof(methodDescription));
            _httpClient = httpClient;
            _paramAppliers = paramAppliers.ToList().AsReadOnly();

            ExpectedCodes.AddRange(_methodDescription.ExpectedStatusCodes);
        }

        protected ApiRequest(ApiRequest<TRes> origin)
            :this(origin._baseUrl, origin._methodDescription, origin._paramAppliers, origin._httpClient)
        {
            RequestModifiers.AddRange(origin.RequestModifiers);
        }

        /// <summary>
        /// Clones an object
        /// </summary>
        public ApiRequest<TRes> Clone()
        {
            return new ApiRequest<TRes>(this);
        }

        /// <summary>
        /// Send request and return serialized response
        /// </summary>
        public async Task<TRes> GetResult(CancellationToken cancellationToken)
        {
            var resp = await SendRequestAsync(cancellationToken);

            await IsStatusCodeUnexpected(resp.Response, true);

            return await ResponseProcessing.DeserializeContent<TRes>(resp.Response.Content);
        }

        /// <summary>
        /// Send request and return detailed information about operation
        /// </summary>
        public async Task<CallDetails<TRes>> GetDetailed(CancellationToken cancellationToken)
        {
            var resp = await SendRequestAsync(cancellationToken);
            var respContent = await ResponseProcessing.DeserializeContent<TRes>(resp.Response.Content);

            var msgDumper = new HttpMessageDumper();
            var reqDump = await msgDumper.Dump(resp.Request);
            var respDump = await msgDumper.Dump(resp.Response);
            var isUnexpectedStatusCode = await IsStatusCodeUnexpected(resp.Response, false);

            return new CallDetails<TRes>
            {
                RequestMessage = resp.Request,
                ResponseMessage = resp.Response,
                ResponseContent = respContent,
                RequestDump = reqDump,
                ResponseDump = respDump,
                StatusCode = resp.Response.StatusCode,
                IsUnexpectedStatusCode = isUnexpectedStatusCode
            };
        }

        async Task<(HttpResponseMessage Response, HttpRequestMessage Request)> SendRequestAsync(CancellationToken cancellationToken)
        {
            var addr = new Uri(_baseUrl.TrimEnd('/') + "/" +  _methodDescription.Url, UriKind.RelativeOrAbsolute);

            var reqMsg = new HttpRequestMessage
            {
                Method = _methodDescription.HttpMethod,
                RequestUri = addr
            };

            ApplyParameters(reqMsg);

            ApplyModifiers(reqMsg);

            var response = await _httpClient.SendAsync(reqMsg, cancellationToken);

            return (response, reqMsg);
        }

        private void ApplyParameters(HttpRequestMessage reqMsg)
        {
            foreach (var pApplier in _paramAppliers)
            {
                pApplier.Apply(reqMsg);
            }
        }

        private void ApplyModifiers(HttpRequestMessage reqMsg)
        {
            foreach (var requestModifier in RequestModifiers)
            {
                try
                {
                    requestModifier?.Modify(reqMsg);
                }
                catch (Exception e)
                {
                    throw new ApiClientException("An error occured while request modifying", e);
                }
            }
        }

        private async Task<bool> IsStatusCodeUnexpected(HttpResponseMessage response, bool throwIfTrue)
        {
            if (response.StatusCode != HttpStatusCode.OK &&
                !ExpectedCodes.Contains(response.StatusCode))
            {
                var contentString = await GetMessageFromResponseContent(response.Content);
                string msg = !string.IsNullOrWhiteSpace(contentString) 
                    ? contentString
                    : response.ReasonPhrase;

                if(throwIfTrue)
                    throw new ResponseCodeException(response.StatusCode, msg);
                else
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<string> GetMessageFromResponseContent(HttpContent responseContent)
        {
            var contentStream = await responseContent.ReadAsStreamAsync();

            using (var rdr = new StreamReader(contentStream))
            {
                var buff = new char[1024];
                var read = await rdr.ReadBlockAsync(buff, 0, 1024);

                return new string(buff, 0, read);
            }
        }
    }
}