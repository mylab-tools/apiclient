using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RedCucumber.Wac
{
    class SupportedResponseProcessors : Collection<IResponseProcessor>
    {
        public SupportedResponseProcessors()
        {
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

    class VoidResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) => returnType == typeof(void);

        public object GetResponse(Task<HttpResponseMessage> message, Type returnType)
        {
            message.Wait();
            return null;
        }
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
            return ExtractPayload(task.Result, returnType);
        }

        protected abstract object ExtractPayload(HttpResponseMessage taskResult, Type returnType);
    }

    class BinaryResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => returnType == typeof(byte[]);
        
        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            return taskResult.Content.ReadAsByteArrayAsync().Result;
        }
    }

    class StringResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => returnType == typeof(string);

        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            return taskResult.Content.ReadAsStringAsync().Result;
        }
    }

    class StructuredObjectResponseProcessor : PayloadBasedResponseProcessor
    {
        public override bool Predicate(Type returnType) => 
            (returnType.IsClass && !returnType.IsAbstract) ||
            (returnType.IsValueType && !returnType.IsPrimitive);

        protected override object ExtractPayload(HttpResponseMessage taskResult, Type returnType)
        {
            var str = taskResult.Content.ReadAsStringAsync().Result.Trim();

            if (str.StartsWith("<"))
                return DeserializeFromXml(str, returnType);

            if (str.StartsWith("{"))
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
}