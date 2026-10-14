using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class BoolContentDeserializer : PrimitiveContentDeserializer<bool>
{
    /// <inheritdoc />
    protected override bool DeserializeCore(string str)
    {
        return Convert.ToBoolean(str);
    }
}