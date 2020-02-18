using System;
using System.Collections.Generic;
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
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly IReadOnlyList<IParameterApplier> _paramAppliers;

        internal ApiRequest(
            string baseUrl,
            MethodDescription methodDescription, 
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider)
        {
            if (paramAppliers == null) throw new ArgumentNullException(nameof(paramAppliers));
            _baseUrl = baseUrl;
            _methodDescription = methodDescription ?? throw new ArgumentNullException(nameof(methodDescription));
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
            _paramAppliers = paramAppliers.ToList().AsReadOnly();

            ExpectedCodes.AddRange(_methodDescription.ExpectedStatusCodes);
        }

        protected ApiRequest(ApiRequest<TRes> origin)
            :this(origin._baseUrl, origin._methodDescription, origin._paramAppliers, origin._httpClientProvider)
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

        public async Task<TRes> Send(CancellationToken cancellationToken)
        {
            var resp = await SendRequestAsync(cancellationToken);

            return await ResponseProcessing.DeserializeContent<TRes>(resp.Content);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(CancellationToken cancellationToken)
        {
            var cl = _httpClientProvider.Provide();

            var reqMsg = new HttpRequestMessage
            {
                Method = _methodDescription.HttpMethod,
            };


            ApplyParameters(reqMsg);

            ApplyModifiers(reqMsg);

            var response = await cl.SendAsync(reqMsg, cancellationToken);

            CheckResponseCode(response);

            return response;
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

        private void CheckResponseCode(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK &&
                !ExpectedCodes.Contains(response.StatusCode))
            {
                throw new ResponseCodeException(response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}