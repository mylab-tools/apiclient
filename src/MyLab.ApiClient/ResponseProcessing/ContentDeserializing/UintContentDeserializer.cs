using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class UintContentDeserializer : PrimitiveContentDeserializer<uint>
{
    /// <inheritdoc />
    protected override uint DeserializeCore(string str)
    {
        return Convert.ToUInt32(str);
    }
}