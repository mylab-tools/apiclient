using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;

namespace MyLab.ApiClient
{
    class GenericCallTaskFactory
    {
        private static readonly MethodInfo CreateGenericTaskMethod;
        private static readonly MethodInfo ApiRequestGetResultMethod;

        private readonly ApiRequestFactory _apiRequestFactory;

        static GenericCallTaskFactory()
        {
            CreateGenericTaskMethod = typeof(ApiRequestFactory)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(m =>
                    m.Name == nameof(ApiRequestFactory.Create) &&
                    m.IsGenericMethod &&
                    m.GetGenericArguments().Length == 1);

            ApiRequestGetResultMethod = typeof(ApiRequest<>).GetMethod("GetResult");
        }

        public GenericCallTaskFactory(ApiRequestFactory apiRequestFactory)
        {
            _apiRequestFactory = apiRequestFactory;
        }

        public object Create(MethodInfo method, object[] args, Type taskResultType)
        {
            var genericMethod = CreateGenericTaskMethod.MakeGenericMethod(taskResultType);
            var apiRequest = genericMethod.Invoke(_apiRequestFactory, new object[]
            {
                method,
                args
            });

            return ApiRequestGetResultMethod.Invoke(apiRequest, new object[]
            {
                CancellationToken.None
            });
        }
    }
}