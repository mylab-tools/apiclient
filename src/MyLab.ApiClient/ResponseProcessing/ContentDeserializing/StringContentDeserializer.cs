using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class StringContentDeserializer : IContentDeserializer
{
    public bool Predicate(Type returnType) => returnType == typeof(string);

    public async Task<object?> DeserializeAsync(HttpContent content, Type returnType)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));

        return await new MediaTypeProc(content)
            .Supports("application/json", async c =>
            {
                var res = await c.ReadAsStringAsync();
                return res.Trim('\"');
            })
            .Supports("text/plain", async c => await c.ReadAsStringAsync())
            .GetResultAsync();
    }
}