using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class ApiRequestFactory<TContract>
    {
        private readonly ServiceDescription _description;
        private readonly IHttpClientProvider _httpClientProvider;
        private readonly ApiRequestFactoryParametersProvider _paramProvider;

        public ApiRequestFactory(IHttpClientProvider httpClientProvider)
        {
            _description = ServiceDescription.Create(typeof(TContract));
            _httpClientProvider = httpClientProvider;
            _paramProvider = new ApiRequestFactoryParametersProvider(_description);
        }
        
        public ApiRequest Create(Expression<Func<TContract, Task>> serviceCallExpr)
        {
            if (serviceCallExpr == null) throw new ArgumentNullException(nameof(serviceCallExpr));
            if (!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");

            var requestParameters = _paramProvider.GetParameters(
                mExpr.Method,
                ExpressionArgumentsToProviders(mExpr));

            return new ApiRequest(
                _description.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClientProvider);
        }

        public ApiRequest<TRes> Create<TRes>(Expression<Func<TContract, Task<TRes>>> serviceCallExpr)
        {
            if (serviceCallExpr == null) throw new ArgumentNullException(nameof(serviceCallExpr));
            if (!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");

            var requestParameters = _paramProvider.GetParameters(
                mExpr.Method,
                ExpressionArgumentsToProviders(mExpr));

            return new ApiRequest<TRes>(
                _description.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClientProvider);
        }

        IApiRequestParameterValueProvider[] ExpressionArgumentsToProviders(MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Arguments
                .Select(a => new ExpressionBasedApiRequestParameterValueProvider(a))
                .Cast<IApiRequestParameterValueProvider>()
                .ToArray();
        }
    }

    class ApiRequestFactory
    {
        public ServiceDescription ServiceDescription { get; }

        private readonly IHttpClientProvider _httpClientProvider;
        private readonly ApiRequestFactoryParametersProvider _paramProvider;

        public ApiRequestFactory(Type contractType, IHttpClientProvider httpClientProvider)
        {
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));
            ServiceDescription = ServiceDescription.Create(contractType);
            _paramProvider = new ApiRequestFactoryParametersProvider(ServiceDescription);
        }
        public ApiRequest Create(MethodInfo method, object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var requestParameters = _paramProvider.GetParameters(
                method,
                ObjectArgumentsToProviders(args));

            return new ApiRequest(
                ServiceDescription.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClientProvider);
        }

        public ApiRequest<TRes> Create<TRes>(MethodInfo method, object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var requestParameters = _paramProvider.GetParameters(
                method,
                ObjectArgumentsToProviders(args));

            return new ApiRequest<TRes>(
                ServiceDescription.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClientProvider);
        }

        IApiRequestParameterValueProvider[] ObjectArgumentsToProviders(object[] args)
        {
            return args
                .Select(a => new SimpleApiRequestParameterValueProvider(a))
                .Cast<IApiRequestParameterValueProvider>()
                .ToArray();
        }

    }

    class ApiRequestFactoryParametersProvider
    {
        private readonly ServiceDescription _serviceDescription;

        public ApiRequestFactoryParametersProvider(ServiceDescription serviceDescription)
        {
            _serviceDescription = serviceDescription;
        }

        public (MethodDescription MethodDescription, IEnumerable<IParameterApplier> Appliers) GetParameters(
            MethodInfo method, IApiRequestParameterValueProvider[] values)
        {
            if (!_serviceDescription.Methods.TryGetValue(method.MetadataToken, out var mDesc))
                throw new ApiClientException("Specified method description not found");

            if (values.Length !=
                mDesc.Parameters.UrlParams.Count +
                mDesc.Parameters.ContentParams.Count +
                mDesc.Parameters.HeaderCollectionParams.Count +
                mDesc.Parameters.HeaderParams.Count
            )
            {
                throw new ApiClientException("ParamAppliers number mismatch");
            }

            var callParams = new List<IParameterApplier>();

            callParams.AddRange(mDesc.Parameters.UrlParams.Select(d =>
                new UrlParameterApplier(d, values[d.Position])));

            callParams.AddRange(mDesc.Parameters.HeaderParams.Select(d =>
                new HeaderParameterApplier(d, values[d.Position])));

            callParams.AddRange(mDesc.Parameters.HeaderCollectionParams.Select(d =>
                new HeaderCollectionParameterApplier(values[d.Position])));

            callParams.AddRange(mDesc.Parameters.ContentParams.Select(d =>
                new ContentParameterApplier(d, values[d.Position])));

            return (mDesc, callParams);
        }
    }
}