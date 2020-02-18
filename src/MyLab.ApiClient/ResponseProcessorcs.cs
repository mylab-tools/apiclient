using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyLab.ApiClient
{
    class SupportedResponseProcessors : Collection<IResponseProcessor>
    {
        public static readonly SupportedResponseProcessors Instance = new SupportedResponseProcessors();

        SupportedResponseProcessors()
        {
            Add(new BoolResponseProcessor());
            Add(new IntResponseProcessor());
            Add(new UintResponseProcessor());
            Add(new DoubleResponseProcessor());
            Add(new StringResponseProcessor());
            Add(new TimeSpanResponseProcessor());
            Add(new DateTimeResponseProcessor());
            Add(new GuidResponseProcessor());
            Add(new BinaryResponseProcessor());
            Add(new StructuredObjectResponseProcessor());
        }
    }

    interface IResponseProcessor
    {
        bool Predicate(Type returnType);

        Task<object> GetResponse(HttpContent content, Type returnType);
    }

    class VoidResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) => returnType == typeof(void);

        /// <inheritdoc />
        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            return await Task.FromResult((object)null);
        }
    }

    class BinaryResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) => returnType == typeof(byte[]);

        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            var bin = await content.ReadAsByteArrayAsync();

            if (bin.Length != 0 && bin[0] == '\"')
            {
                var base64Str = Encoding.UTF8.GetString(bin).Trim('\"');
                return Convert.FromBase64String(base64Str);
            }

            return bin;
        }
    }

    class StringResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) => returnType == typeof(string);

        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            var res = await content.ReadAsStringAsync();
            return res.Trim('\"');
        }
    }

    class StructuredObjectResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) =>
            (returnType.IsClass && !returnType.IsAbstract) ||
            (returnType.IsValueType && !returnType.IsPrimitive);

        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            var contentStr = await content.ReadAsStringAsync();
            var str = contentStr.Trim(' ', '\"');

            if (str == "null")
                return null;

            if (str.StartsWith("<"))
                return DeserializeFromXml(str, returnType);

            if (str.StartsWith("{") || str.StartsWith("["))
                return DeserializeFromJson(str, returnType);

            throw new ApiClientException("Unexpected response payload content. Only XML and JSON are supported for structural object.");
        }

        private object DeserializeFromJson(string str, Type returnType)
        {
            var d = new JsonSerializer();

            using (var r = new StringReader(str))
            {
                return d.Deserialize(r, returnType);
            }
        }

        private object DeserializeFromXml(string str, Type returnType)
        {
            var rootAttribute = returnType.GetTypeInfo().GetCustomAttribute<XmlRootAttribute>();

            if (rootAttribute == null)
            {
                using (var strReader = new StringReader(str))
                using (var xmlReader = new XmlTextReader(strReader))
                {
                    xmlReader.MoveToContent();
                    rootAttribute = new XmlRootAttribute(xmlReader.Name);
                }
            }

            var d = new XmlSerializer(returnType, rootAttribute);

            using (var r = new StringReader(str))
            {
                return d.Deserialize(r);
            }
        }
    }

    abstract class PrimitiveResponseProcessor<T> : IResponseProcessor
        where T : struct
    {
        /// <inheritdoc />
        public bool Predicate(Type returnType)
        {
            return returnType == typeof(T);
        }

        /// <inheritdoc />
        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            var contentStr = await content.ReadAsStringAsync();
            return Deserialize(contentStr.Trim('\"', ' '));
        }

        protected abstract T Deserialize(string str);
    }

    class UintResponseProcessor : PrimitiveResponseProcessor<uint>
    {
        /// <inheritdoc />
        protected override uint Deserialize(string str)
        {
            return Convert.ToUInt32(str);
        }
    }

    class IntResponseProcessor : PrimitiveResponseProcessor<int>
    {
        /// <inheritdoc />
        protected override int Deserialize(string str)
        {
            return Convert.ToInt32(str);
        }
    }

    class DoubleResponseProcessor : PrimitiveResponseProcessor<double>
    {
        /// <inheritdoc />
        protected override double Deserialize(string str)
        {
            return Convert.ToDouble(str.Replace(",","."), CultureInfo.InvariantCulture);
        }
    }

    class BoolResponseProcessor : PrimitiveResponseProcessor<bool>
    {
        /// <inheritdoc />
        protected override bool Deserialize(string str)
        {
            return Convert.ToBoolean(str);
        }
    }

    class TimeSpanResponseProcessor : PrimitiveResponseProcessor<TimeSpan>
    {
        /// <inheritdoc />
        protected override TimeSpan Deserialize(string str)
        {
            return TimeSpan.Parse(str);
        }
    }

    class DateTimeResponseProcessor : PrimitiveResponseProcessor<DateTime>
    {
        /// <inheritdoc />
        protected override DateTime Deserialize(string str)
        {
            return DateTime.Parse(str, CultureInfo.InvariantCulture);
        }
    }

    class GuidResponseProcessor : PrimitiveResponseProcessor<Guid>
    {
        /// <inheritdoc />
        protected override Guid Deserialize(string str)
        {
            return Guid.Parse(str);
        }
    }
}
