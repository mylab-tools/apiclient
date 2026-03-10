using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class StructuredObjectContentDeserializer : IContentDeserializer
{
    public bool Predicate(Type returnType)
    {
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));

        return returnType is { IsClass: true, IsAbstract: false } ||
               returnType is { IsValueType: true, IsPrimitive: false };
    }
    

    public async Task<object?> DeserializeAsync(HttpContent content, Type returnType)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));

        return await new MediaTypeProc(content)
            .Supports("application/json", async c => await JsonDeserializationTools.ReadObjectJson(c, returnType))
            .Supports("application/xml", async c => await XmlDeserializationTools.ReadObjectXml(c, returnType))
            .GetResult();
    }
}