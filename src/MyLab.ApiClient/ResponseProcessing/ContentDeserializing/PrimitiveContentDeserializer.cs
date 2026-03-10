using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

abstract class PrimitiveContentDeserializer<T> : IContentDeserializer
    where T : struct
{
    /// <inheritdoc />
    public bool Predicate(Type returnType)
    {
        return returnType == typeof(T);
    }

    /// <inheritdoc />
    public async Task<object?> DeserializeAsync(HttpContent content, Type returnType)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));

        var contentStr = await content.ReadAsStringAsync();
        return await new MediaTypeProc(content)
            .Supports("application/json",_ => Task.FromResult<object?>(DeserializeCore(contentStr.Trim('\"', ' '))))
            .Supports("text/plain", _ => Task.FromResult<object?>(DeserializeCore(contentStr.Trim('\"', ' '))))
            .GetResult();
    }

    protected abstract T DeserializeCore(string str);
}