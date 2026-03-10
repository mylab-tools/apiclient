using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class IntContentDeserializer : PrimitiveContentDeserializer<int>
{
    /// <inheritdoc />
    protected override int DeserializeCore(string str)
    {
        return Convert.ToInt32(str);
    }
}