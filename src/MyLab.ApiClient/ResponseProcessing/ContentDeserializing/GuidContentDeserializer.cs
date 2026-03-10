using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class GuidContentDeserializer : PrimitiveContentDeserializer<Guid>
{
    /// <inheritdoc />
    protected override Guid DeserializeCore(string str)
    {
        return Guid.Parse(str);
    }
}