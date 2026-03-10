using System;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class TimeSpanContentDeserializer : PrimitiveContentDeserializer<TimeSpan>
{
    /// <inheritdoc />
    protected override TimeSpan DeserializeCore(string str)
    {
        return TimeSpan.Parse(str);
    }
}