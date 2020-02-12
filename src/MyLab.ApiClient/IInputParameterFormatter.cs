using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Formats input parameter before place it into request
    /// </summary>
    public interface IInputParameterFormatter
    {
        string Format(object value);
    }

    public class StringParameterFormatter : IInputParameterFormatter
    {
        public string Format(object value)
        {
            return value?.ToString();
        }
    }

    public class JsonParameterFormatter : IInputParameterFormatter
    {
        public string Format(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }

    public class XmlParameterFormatter : IInputParameterFormatter
    {
        public string Format(object value)
        {
            if (value == null)
                return null;
            var serializer = new XmlSerializer(value.GetType());

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var strBuilder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(strBuilder, settings))
            {
                serializer.Serialize(xmlWriter, value, ns);
            }

            return strBuilder.ToString();
        }
    }

    public class UrlFormParameterFormatter : IInputParameterFormatter
    {
        public string Format(object value)
        {
            if (value == null) return string.Empty;

            var tp = value.GetType();

            CheckValueType(tp);

            var properties = tp
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .Select(p =>
                {
                    var val = JsonConvert.SerializeObject(p.GetValue(value));
                    var encVal = HttpUtility.UrlEncode(val.Trim('\"'));
                    return $"{p.Name}={encVal}";
                });

            var resultStr = string.Join("&", properties.ToArray());

            return resultStr;
        }

        private static void CheckValueType(Type tp)
        {
            if (tp.IsPrimitive)
                throw new NotSupportedException("Primitive parameter not supported for UrlForm formatting");
            if (tp == typeof(Guid) ||
                tp == typeof(string) ||
                tp == typeof(DateTime) ||
                tp == typeof(TimeSpan))
                throw new NotSupportedException($"Parameter type '{tp.FullName}' not supported for UrlForm formatting");
        }
    }
}