using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DotApiClient
{
    class SupportedResponseProcessors : Collection<IResponseProcessor>
    {
        public SupportedResponseProcessors()
        {
            Add(new IntResponseProcessor());
            Add(new UintResponseProcessor());
            Add(new DoubleResponseProcessor());
            Add(new VoidResponseProcessor());
            Add(new TaskResponseProcessor());
            Add(new StringResponseProcessor());
            Add(new BinaryResponseProcessor());
            Add(new StructuredObjectResponseProcessor());
        }
    }

    interface IResponseProcessor
    {
        bool Predicate(Type returnType);

        object GetResponse(Task<HttpResponseMessage> task, Type returnType);
    }

    class TaskResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) 
            => returnType == typeof(Task) || returnType == typeof(Task<HttpResponseMessage>);

        public object GetResponse(Task<HttpResponseMessage> task, Type returnType) => task;
    }

    abstract class PayloadBasedResponseProcessor : IResponseProcessor
    {
        public abstract bool Predicate(Type returnType);
        
        public object GetResponse(Task<HttpResponseMessage> task, Type returnType)
        {
            task.Wait();

            int statusCodeIndex = (int)task.Result.StatusCode;
            if (statusCodeIndex < 200 || statusCodeIndex >= 300)
                throw new WrongHttpStatusCodeException(task.Result);

            return ExtractPayload(task.Result, returnType);
        }

        protected abstract object ExtractPayload(HttpResponseMessage taskResult, Type returnType);
    }

    class VoidResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => returnType == typeof(void);

        /// <inheritdoc />
        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            return null;
        }
    }

    class BinaryResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => returnType == typeof(byte[]);
        
        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            var bin = taskResult.Content.ReadAsByteArrayAsync().Result;

            if (bin.Length != 0 && bin[0] == '\"')
            {
                var base64Str = Encoding.UTF8.GetString(bin).Trim('\"');
                return Convert.FromBase64String(base64Str);
            }

            return bin;
        }
    }

    class StringResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => returnType == typeof(string);

        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            return taskResult.Content.ReadAsStringAsync().Result.Trim('\"');
        }
    }

    class StructuredObjectResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => 
            (returnType.IsClass && !returnType.IsAbstract) ||
            (returnType.IsValueType && !returnType.IsPrimitive);

        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            var str = taskResult.Content.ReadAsStringAsync().Result.Trim(' ', '\"');

            if (str == "null")
                return null;

            if (str.StartsWith("<"))
                return DeserializeFromXml(str, returnType);

            if (str.StartsWith("{") || str.StartsWith("["))
                return DeserializeFromJson(str, returnType);

            throw new ResponseProcessingException("Unexpexted response pyload content. Only XML and JSON supported for structural object.");
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

    abstract class PrimitiveResponseProcessor<T> : PayloadBasedResponseProcessor
        where T : struct 
    {
        /// <inheritdoc />
        public override bool Predicate(Type returnType)
        {
            return returnType == typeof(T);
        }

        /// <inheritdoc />
        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            return Deserialize(taskResult.Content.ReadAsStringAsync().Result.Trim('\"', ' '));
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
            return Convert.ToDouble(str);
        }
    }
}