using System;
using System.Reflection;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides config api key
    /// </summary>
    static class ApiConfigKeyProvider
    {
        public static string Provide(Type apiContract)
        {
            if (apiContract == null) throw new ArgumentNullException(nameof(apiContract));

            if (!apiContract.IsInterface)
                throw new NotSupportedException("Only interfaces are supported");

            var apiAttr = apiContract.GetCustomAttribute<ApiAttribute>();

            return apiAttr?.Key ?? apiContract.Name;
        }
    }
}
