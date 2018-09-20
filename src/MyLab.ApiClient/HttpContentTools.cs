using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyLab.ApiClient
{
    static class HttpContentTools
    {
        public static object ExtractResult(HttpResponseMessage resp, Type methodReturnType)
        {
            var proc = SupportedResponseProcessors.Instance.FirstOrDefault(p => p.Predicate(methodReturnType));
            if (proc == null)
                throw new ResponseProcessingException(
                    $"Response processor not found for return type '{methodReturnType.FullName}'");

            return proc.GetResponse(resp, methodReturnType);
        }

        public static HttpContent CreateContent(object val, string mimeType)
        {
            if (val == null) throw new ArgumentNullException(nameof(val), "Request body content is null");

            var encoding = new UTF8Encoding(false);

            switch (mimeType)
            {
                case "application/octet-stream":
                    {
                        var content = new ByteArrayContent((byte[])val);
                        content.Headers.Add("Content-Type", mimeType);
                        return content;
                    }
                case "application/json":
                    return new StringContent(PayloadToJson(val), encoding, mimeType);
                case "application/xml":
                    return new StringContent(PayloadToXml(val, encoding), encoding, mimeType);
                default:
                    return new StringContent(val.ToString(), encoding, mimeType);
            }
        }

        private static string PayloadToJson(object payload)
        {
            var jsonS = new JsonSerializer();

            using (StringWriter textWriter = new StringWriter())
            {
                jsonS.Serialize(textWriter, payload);
                return textWriter.ToString();
            }
        }

        private static string PayloadToXml(object payload, Encoding encoding)
        {
            var ser = new XmlSerializer(payload.GetType());

            using (var mem = new MemoryStream())
            using (var textWriter = XmlWriter.Create(mem, new XmlWriterSettings
            {
                Encoding = encoding,
            }))
            {
                ser.Serialize(textWriter, payload);
                return Encoding.UTF8.GetString(mem.ToArray());
            }
        }
    }
}