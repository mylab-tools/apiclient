using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class XmlHttpContentFactory : IHttpContentFactory
{
    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        string strContent;

        if (source != null)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var ser = new XmlSerializer(source.GetType());
            var xmlSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = new UTF8Encoding(false)
            };
            using (var mem = new MemoryStream())
            using (var wrtr = XmlWriter.Create(mem, xmlSettings))
            {

                ser.Serialize(wrtr, source, ns);
                wrtr.Flush();

                strContent = Encoding.UTF8.GetString(mem.ToArray());
            }
        }
        else
        {
            strContent = string.Empty;
        }

        var content = new StringContent(strContent)
        {
            Headers = { ContentType = new MediaTypeHeaderValue("application/xml") }
        };

        //To calculate `content-length` header
        var len = content.Headers.ContentLength.GetValueOrDefault();

        return content;
    }
}