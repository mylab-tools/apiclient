using MyLab.ApiClient.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class BinaryContentDeserializer : IContentDeserializer
{
    public bool Predicate(Type returnType) => returnType == typeof(byte[]);

    public async Task<object?> DeserializeAsync(HttpContent content, Type returnType)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));
        
        return await new MediaTypeProc(content)
            .Supports("application/json", async c =>
            {
                var bin = await c.ReadAsByteArrayAsync();
                if (bin.Length != 0 && bin[0] == '\"')
                {
                    var base64Str = Encoding.UTF8.GetString(bin).Trim('\"');
                    return Convert.FromBase64String(base64Str);
                }

                return bin;
            })
            //.Supports("application/octet-stream", async c => await c.ReadAsByteArrayAsync())
            .Default(async c => await c.ReadAsByteArrayAsync())
            .GetResult();
    }
}