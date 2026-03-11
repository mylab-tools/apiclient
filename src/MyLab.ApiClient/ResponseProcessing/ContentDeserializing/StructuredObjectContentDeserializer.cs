using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class StructuredObjectContentDeserializer : IContentDeserializer
{
    readonly JsonDeserializationTools _jsonSerTools;
    readonly XmlDeserializationTools _xmlSerTools;
    public StructuredObjectContentDeserializer(JsonDeserializationTools jsonSerTools,
        XmlDeserializationTools xmlSerTools)
    {
        _jsonSerTools = jsonSerTools ?? throw new ArgumentNullException(nameof(jsonSerTools));
        _xmlSerTools = xmlSerTools ?? throw new ArgumentNullException(nameof(xmlSerTools));
    }
    
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
            .Supports("application/json", async c => await _jsonSerTools.ReadObjectJson(c, returnType))
            .Supports("application/xml", async c => await _xmlSerTools.ReadObjectXml(c, returnType))
            .GetResult();
    }
}