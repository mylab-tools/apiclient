using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class EnumerableContentDeserializer : IContentDeserializer
{
    public async Task<object?> DeserializeAsync(HttpContent content, Type returnType)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));

        var listType = typeof(List<>).MakeGenericType(returnType.GetGenericArguments());

        return await new MediaTypeProc(content)
            .Supports("application/json", async c => await JsonDeserializationTools.ReadObjectJson(c, listType))
            .Supports("application/xml", async c => await XmlDeserializationTools.ReadObjectXml(c, listType))
            .GetResult();
    }

    public bool Predicate(Type returnType)
    {
        if (returnType == null) throw new ArgumentNullException(nameof(returnType));
        
        return returnType.IsGenericType &&
               returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
    }
}