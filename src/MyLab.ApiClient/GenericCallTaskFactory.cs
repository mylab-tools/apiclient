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

        private readonly ApiRequestFactory _apiRequestFactory;

        static GenericCallTaskFactory()
        {
            CreateGenericTaskMethod = typeof(ApiRequestFactory)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(m =>
                    m.Name == nameof(ApiRequestFactory.Create) &&
                    m.IsGenericMethod &&
                    m.GetGenericArguments().Length == 1);
        }

        public GenericCallTaskFactory(ApiRequestFactory apiRequestFactory)
        {
            _apiRequestFactory = apiRequestFactory;
        }

        public object CreateCall(MethodInfo method, object[] args, Type taskResultType)
        {
            //ApiRequest<STRING>.GetResultAsync - just for compilling
            return CreateCore(nameof(ApiRequest<string>.GetResultAsync), method, args, taskResultType);
        }

        public object CreateDetailed(MethodInfo method, object[] args, Type taskResultType)
        {
            //ApiRequest<STRING>.GetResultAsync - just for compilling
            return CreateCore(nameof(ApiRequest<string>.GetDetailedAsync), method, args, taskResultType);
        }

        object CreateCore(string requestMethodName, MethodInfo method, object[] args, Type taskResultType)
        {
            var createTaskMethod = CreateGenericTaskMethod.MakeGenericMethod(taskResultType);
            var apiRequest = createTaskMethod.Invoke(_apiRequestFactory, new object[]
            {
                method,
                args
            });

            var getResultMethod = apiRequest.GetType().GetMethod(requestMethodName);
            if (getResultMethod == null)
                throw new InvalidOperationException($"Cant find method '{requestMethodName}' in type '{apiRequest.GetType().FullName}'");

            return getResultMethod.Invoke(apiRequest, new object[]
            {
                CancellationToken.None
            });
        }
    }
}