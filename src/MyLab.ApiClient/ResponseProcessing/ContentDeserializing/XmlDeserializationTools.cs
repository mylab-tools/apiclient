using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class XmlDeserializationTools
{
    static readonly XmlReaderSettings SecureReaderSettings = new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null
    };

    public async Task<object?> ReadObjectXml(HttpContent content, Type returnType)
    {
        var contentStr = await content.ReadAsStringAsync();
        var str = contentStr.Trim(' ', '\"');

        if (str == "null")
            return null;

        return DeserializeFromXml(str, returnType);
    }

    object? DeserializeFromXml(string str, Type returnType)
    {
        var rootAttribute = returnType.GetTypeInfo().GetCustomAttribute<XmlRootAttribute>()
                            ?? new XmlRootAttribute(ReadRootElementName(str));

        var serializer = new XmlSerializer(returnType, rootAttribute);

        using var stringReader = new StringReader(str);
        using var xmlReader = XmlReader.Create(stringReader, SecureReaderSettings);
        return serializer.Deserialize(xmlReader);
    }

    static string ReadRootElementName(string xml)
    {
        using var stringReader = new StringReader(xml);
        using var xmlReader = XmlReader.Create(stringReader, SecureReaderSettings);

        xmlReader.MoveToContent();
        return xmlReader.Name;
    }
}
