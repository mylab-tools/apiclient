using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;

namespace MyLab.ApiClient
{
    class ApiRequestFactory<TContract>
    {
        private readonly ServiceDescription _description;

        private readonly HttpClient _httpClient;
        
        public ApiRequestFactory(ServiceDescription description, HttpClient httpClient)
        {
            _description = description;
            _httpClient = httpClient;
        }
        
        public ApiRequest<string> Create(Expression<Action<TContract>> serviceCallExpr)
        {
            if (serviceCallExpr == null) throw new ArgumentNullException(nameof(serviceCallExpr));
            if (!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");

            var requestParameters = GetParameters(
                mExpr.Method,
                ExpressionArgumentsToProviders(mExpr));

            return new ApiRequest<string>(
                _description.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClient);
        }

        public ApiRequest<TRes> Create<TRes>(Expression<Func<TContract, TRes>> serviceCallExpr)
        {
            if (serviceCallExpr == null) throw new ArgumentNullException(nameof(serviceCallExpr));
            if (!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");

            var requestParameters = GetParameters(
                mExpr.Method,
                ExpressionArgumentsToProviders(mExpr));

            return new ApiRequest<TRes>(
                _description.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClient);
        }

        public ApiRequest<string> Create(MethodInfo method, object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var requestParameters = GetParameters(
                method, 
                ObjectArgumentsToProviders(args));

            return new ApiRequest<string>(
                _description.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClient);
        }

        public ApiRequest<TRes> Create<TRes>(MethodInfo method, object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var requestParameters = GetParameters(
                method,
                ObjectArgumentsToProviders(args));

            return new ApiRequest<TRes>(
                _description.Url,
                requestParameters.MethodDescription,
                requestParameters.Appliers,
                _httpClient);
        }

        (MethodDescription MethodDescription, IEnumerable<IParameterApplier> Appliers) GetParameters(
            MethodInfo method, IApiRequestParameterValueProvider[] values)
        {
            if (!_description.Methods.TryGetValue(method.MetadataToken, out var mDesc))
                throw new ApiClientException("Specified method description not found");

            if (values.Length !=
                    mDesc.Parameters.UrlParams.Count +
                    mDesc.Parameters.ContentParams.Count +
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

            callParams.AddRange(mDesc.Parameters.ContentParams.Select(d =>
                new ContentParameterApplier(d, values[d.Position])));

            return (mDesc, callParams);
        }

        IApiRequestParameterValueProvider[] ExpressionArgumentsToProviders(MethodCallExpression methodCallExpression)
        {
            return methodCallExpression.Arguments
                .Select(a => new ExpressionBasedApiRequestParameterValueProvider(a))
                .Cast<IApiRequestParameterValueProvider>()
                .ToArray();
        }

        IApiRequestParameterValueProvider[] ObjectArgumentsToProviders(object[] args)
        {
            return args
                .Select(a => new SimpleApiRequestParameterValueProvider(a))
                .Cast<IApiRequestParameterValueProvider>()
                .ToArray();
        }

    }
}