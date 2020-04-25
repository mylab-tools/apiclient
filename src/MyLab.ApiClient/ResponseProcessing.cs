using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains response processing logic
    /// </summary>
    static class ResponseProcessing
    {
        public static async Task<object> DeserializeContent(Type targetType, HttpContent httpContent)
        {
            if (targetType == typeof(void))
                return null;
            if (httpContent.Headers.ContentLength.HasValue && httpContent.Headers.ContentLength.Value == 0)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            var proc = SupportedResponseProcessors.Instance.FirstOrDefault(p => p.Predicate(targetType));
            if (proc == null)
                throw new ApiClientException(
                    $"Response processor not found for return type '{targetType.FullName}'");

            return await proc.GetResponse(httpContent, targetType);
        }
        public static async Task<T> DeserializeContent<T>(HttpContent httpContent)
        {
            return (T) await DeserializeContent(typeof(T), httpContent);
        }
    }
}