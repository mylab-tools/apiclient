using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class ApiRequestFactory<TContract>
    {
        private readonly ServiceDescription _srvDesc;
        private readonly IHttpClientProvider _httpClientProvider;

        public RequestFactoringSettings Settings { get; set; }

        public ApiRequestFactory(IHttpClientProvider httpClientProvider)
        {
            _srvDesc = ServiceDescription.Create(typeof(TContract));
            _httpClientProvider = httpClientProvider;
        }
        
        public ApiRequest Create(Expression<Func<TContract, Task>> serviceCallExpr)
        {
            if (serviceCallExpr == null) throw new ArgumentNullException(nameof(serviceCallExpr));
            if (!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");

            var ctx = CreateFactoringCtx(mExpr);

            return new ApiRequest(_srvDesc.Url, ctx, _httpClientProvider);
        }

        public ApiRequest<TRes> Create<TRes>(Expression<Func<TContract, Task<TRes>>> serviceCallExpr)
        {
            if (serviceCallExpr == null) throw new ArgumentNullException(nameof(serviceCallExpr));
            if (!(serviceCallExpr.Body is MethodCallExpression mExpr))
                throw new NotSupportedException("Only method calls are supported");

            var ctx = CreateFactoringCtx(mExpr);

            return new ApiRequest<TRes>(_srvDesc.Url, ctx, _httpClientProvider);
        }

        ApiRequestFactoryContext CreateFactoringCtx(MethodCallExpression methodCallExpression)
        {
            var mDesc = _srvDesc.GetRequiredMethod(methodCallExpression.Method);
            var argProviders = ExpressionArgumentsToProviders(methodCallExpression);

            return new ApiRequestFactoryContext(mDesc, argProviders)
            {
                Settings = Settings
            };
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
        private readonly IHttpClientProvider _httpClientProvider;

        public ServiceDescription ServiceDescription { get; private set; }

        public RequestFactoringSettings Settings { get; set; }

        public ApiRequestFactory(Type contractType, IHttpClientProvider httpClientProvider)
        {
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            
            _httpClientProvider = httpClientProvider ?? throw new ArgumentNullException(nameof(httpClientProvider));

            ServiceDescription = ServiceDescription.Create(contractType);
        }
        public ApiRequest Create(MethodInfo method, object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            var ctx = CreateFactoringCtx(method, args);

            return new ApiRequest(ServiceDescription.Url, ctx, _httpClientProvider);
        }

        public ApiRequest<TRes> Create<TRes>(MethodInfo method, object[] args)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            
            var ctx = CreateFactoringCtx(method, args);
            
            return new ApiRequest<TRes>(ServiceDescription.Url, ctx, _httpClientProvider);
        }

        ApiRequestFactoryContext CreateFactoringCtx(MethodInfo method, object[] args)
        {
            var mDesc = ServiceDescription.GetRequiredMethod(method);
            var argProviders = ObjectArgumentsToProviders(args);

            return new ApiRequestFactoryContext(mDesc, argProviders)
            {
                Settings = Settings
            };
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