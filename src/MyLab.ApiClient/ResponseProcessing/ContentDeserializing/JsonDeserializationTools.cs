using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

static class JsonDeserializationTools
{
    public static async Task<object?> ReadObjectJson(HttpContent content, Type returnType)
    {
        var contentStr = await content.ReadAsStringAsync();
        var str = contentStr.Trim(' ', '\"');

        if (str == "null")
            return null;

        return DeserializeFromJson(str, returnType);
    }

    static object? DeserializeFromJson(string str, Type returnType)
    {
        var d = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        string resultStr;

        if (returnType.IsEnum && str.Length > 1 && str[0] != '\"' && str[^1] != '\"')
        {
            resultStr = $"\"{str}\"";
        }
        else
        {
            resultStr = str;
        }

        using var r = new StringReader(resultStr);
            
        return d.Deserialize(r, returnType);
    }
}