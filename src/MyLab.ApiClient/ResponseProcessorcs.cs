using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class SupportedResponseProcessors : Collection<IResponseProcessor>
    {
        public static readonly SupportedResponseProcessors Instance = new SupportedResponseProcessors();

        SupportedResponseProcessors()
        {
            Add(new BoolResponseProcessor());
            Add(new ShortResponseProcessor());
            Add(new UShortResponseProcessor());
            Add(new IntResponseProcessor());
            Add(new UintResponseProcessor());
            Add(new LongResponseProcessor());
            Add(new ULongResponseProcessor());
            Add(new DoubleResponseProcessor());
            Add(new FloatResponseProcessor());
            Add(new DecimalResponseProcessor());
            Add(new StringResponseProcessor());
            Add(new TimeSpanResponseProcessor());
            Add(new DateTimeResponseProcessor());
            Add(new GuidResponseProcessor());
            Add(new BinaryResponseProcessor());
            Add(new StructuredObjectResponseProcessor());
            Add(new EnumerableResponseProcessor());
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
            return await new MediaTypeProc(content)
                .Supports("application/json", async c =>
                {
                    var bin = await c.ReadAsByteArrayAsync();
                    if (bin.Length != 0 && bin[0] == '\"')
                    {
                        var base64Str = Encoding.UTF8.GetString(bin).Trim('\"');
                        return Convert.FromBase64String(base64Str);
                    }

                    return bin;
                })
                .Supports("application/octet-stream", async c => await c.ReadAsByteArrayAsync())
                .GetResult();
        }
    }

    class StringResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) => returnType == typeof(string);

        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            return await new MediaTypeProc(content)
                .Supports("application/json", async c =>
                {
                    var res = await content.ReadAsStringAsync();
                    return res.Trim('\"');
                })
                .Supports("text/plain", async c => await content.ReadAsStringAsync())
                .GetResult();
        }
    }

    class EnumerableResponseProcessor : IResponseProcessor
    {
        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            var listType = typeof(List<>).MakeGenericType(returnType.GetGenericArguments());

            return await new MediaTypeProc(content)
                .Supports("application/json", async c => await ResponseProcessorConverter.ReadObjectJson(content, listType))
                .Supports("application/xml", async c => await ResponseProcessorConverter.ReadObjectXml(content, listType))
                .GetResult();
        }

        public bool Predicate(Type returnType)
        {
            return returnType.IsGenericType &&
                   returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }
    }

    class StructuredObjectResponseProcessor : IResponseProcessor
    {
        public bool Predicate(Type returnType) =>
            (returnType.IsClass && !returnType.IsAbstract) ||
            (returnType.IsValueType && !returnType.IsPrimitive);

        public async Task<object> GetResponse(HttpContent content, Type returnType)
        {
            return await new MediaTypeProc(content)
                .Supports("application/json", async c => await ResponseProcessorConverter.ReadObjectJson(content, returnType))
                .Supports("application/xml", async c => await ResponseProcessorConverter.ReadObjectXml(content, returnType))
                .GetResult();
            
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
            return await new MediaTypeProc(content)
                .Supports("application/json", async c => Deserialize(contentStr.Trim('\"', ' ')))
                .Supports("text/plain", async c => Deserialize(contentStr.Trim('\"', ' ')))
                .GetResult();
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
            return double.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
        }
    }

    class FloatResponseProcessor : PrimitiveResponseProcessor<float>
    {
        /// <inheritdoc />
        protected override float Deserialize(string str)
        {
            return float.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
        }
    }

    class DecimalResponseProcessor : PrimitiveResponseProcessor<decimal>
    {
        /// <inheritdoc />
        protected override decimal Deserialize(string str)
        {
            return decimal.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
        }
    }

    class ShortResponseProcessor : PrimitiveResponseProcessor<short>
    {
        /// <inheritdoc />
        protected override short Deserialize(string str)
        {
            return short.Parse(str);
        }
    }

    class UShortResponseProcessor : PrimitiveResponseProcessor<ushort>
    {
        /// <inheritdoc />
        protected override ushort Deserialize(string str)
        {
            return ushort.Parse(str);
        }
    }

    class LongResponseProcessor : PrimitiveResponseProcessor<long>
    {
        /// <inheritdoc />
        protected override long Deserialize(string str)
        {
            return long.Parse(str);
        }
    }

    class ULongResponseProcessor : PrimitiveResponseProcessor<ulong>
    {
        /// <inheritdoc />
        protected override ulong Deserialize(string str)
        {
            return ulong.Parse(str);
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
