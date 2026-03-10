using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

static class XmlDeserializationTools
{
    public static async Task<object?> ReadObjectXml(HttpContent content, Type returnType)
    {
        var contentStr = await content.ReadAsStringAsync();
        var str = contentStr.Trim(' ', '\"');

        if (str == "null")
            return null;

        return DeserializeFromXml(str, returnType);
    }

    static object? DeserializeFromXml(string str, Type returnType)
    {
        var rootAttribute = returnType.GetTypeInfo().GetCustomAttribute<XmlRootAttribute>();

        if (rootAttribute == null)
        {
            using var strReader = new StringReader(str);
            using var xmlReader = new XmlTextReader(strReader);
            xmlReader.MoveToContent();
            rootAttribute = new XmlRootAttribute(xmlReader.Name);
        }

        var d = new XmlSerializer(returnType, rootAttribute);

        using var r = new StringReader(str);
        return d.Deserialize(r);
    }
}