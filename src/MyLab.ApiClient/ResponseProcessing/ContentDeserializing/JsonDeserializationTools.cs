using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient.JsonSerialization;
using Newtonsoft.Json;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class JsonDeserializationTools
{
    readonly IJsonSerializer _jsonSerializer;

    public JsonDeserializationTools(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
    }
    
    public async Task<object?> ReadObjectJson(HttpContent content, Type returnType)
    {
        var contentStr = await content.ReadAsStringAsync();
        var str = contentStr.Trim(' ', '\"');

        if (str == "null")
            return null;

        return DeserializeFromJson(str, returnType);
    }

    object? DeserializeFromJson(string str, Type returnType)
    {
        string resultStr;

        if (returnType.IsEnum && str.Length > 1 && str[0] != '\"' && str[^1] != '\"')
        {
            resultStr = $"\"{str}\"";
        }
        else
        {
            resultStr = str;
        }
        
        return _jsonSerializer.Deserialize(resultStr, returnType);
    }
}