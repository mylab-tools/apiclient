using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TestServer.Models
{
    [JsonConverter(typeof(StringEnumConverter))] 
    public enum TestEnum
    {
        Undefined,
        Value1,
        [EnumMember(Value = "val-2")]
        Value2
    }
}
