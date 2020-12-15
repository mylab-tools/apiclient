using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    public class ApiRequest<TRes> : ApiRequestBase<CallDetails<TRes>>
    {
        private readonly Type _returnType;
        
        internal ApiRequest(
            string baseUrl,
            MethodDescription methodDescription,
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider,
            Type returnType = null)
            :base(baseUrl, methodDescription, paramAppliers, httpClientProvider)
        {
            _returnType = returnType ?? typeof(TRes);
            if (!typeof(TRes).IsAssignableFrom(_returnType))
                throw new InvalidOperationException($"Specified return type '{_returnType.FullName}' must be assignable to method return type '{typeof(TRes).FullName}'");
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiRequest{TRes}"/>
        /// </summary>
        protected ApiRequest(ApiRequest<TRes> origin)
            : base(origin)
        {
            _returnType = origin._returnType;
        }

        /// <inheritdoc />
        public override async Task<CallDetails<TRes>> GetDetailedAsync(CancellationToken cancellationToken)
        {
            var details = await base.GetDetailedAsync(cancellationToken);

            object respContent;

            try
            {
                respContent = await ResponseProcessing.DeserializeContent(
                    _returnType,
                    details.ResponseMessage.Content,
                    details.ResponseMessage.StatusCode);
            }
            catch (Exception e)
            {
                throw new DetailedResponseProcessingException<CallDetails<TRes>>(details, e);
            }

            details.ResponseContent = (TRes)respContent;

            return details;
        }

        /// <summary>
        /// Send request and return serialized response
        /// </summary>
        public async Task<TRes> GetResultAsync(CancellationToken cancellationToken)
        {
            var resp = await GetResponseAsync(cancellationToken);

            TRes resultObject;

            try
            {
                resultObject = (TRes)await ResponseProcessing.DeserializeContent(
                    _returnType,
                    resp.Content,
                    resp.StatusCode);
            }
            catch (Exception e)
            {
                throw new ResponseProcessingException(e);
            }

            return resultObject;
        }
    }

    public class ApiRequest : ApiRequestBase<CallDetails>
    {
        internal ApiRequest(
            string baseUrl,
            MethodDescription methodDescription,
            IEnumerable<IParameterApplier> paramAppliers,
            IHttpClientProvider httpClientProvider)
            : base(baseUrl, methodDescription, paramAppliers, httpClientProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiRequest{TRes}"/>
        /// </summary>
        protected ApiRequest(ApiRequest origin)
            : base(origin)
        {
        }

        /// <summary>
        /// Calls API without result
        /// </summary>
        public async Task CallAsync(CancellationToken cancellationToken)
        {
            await GetResponseAsync(cancellationToken);
        }
    }
}