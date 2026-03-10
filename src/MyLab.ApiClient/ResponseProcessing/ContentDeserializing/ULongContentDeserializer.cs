namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class ULongContentDeserializer : PrimitiveContentDeserializer<ulong>
{
    /// <inheritdoc />
    protected override ulong DeserializeCore(string str)
    {
        return ulong.Parse(str);
    }
}