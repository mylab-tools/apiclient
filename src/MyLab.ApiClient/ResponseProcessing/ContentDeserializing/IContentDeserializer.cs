using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing
{
    interface IContentDeserializer
    {
        bool Predicate(Type returnType);

        Task<object?> DeserializeAsync(HttpContent content, Type returnType);
    }
}
