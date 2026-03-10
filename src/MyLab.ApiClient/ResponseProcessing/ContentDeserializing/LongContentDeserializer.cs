namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

 class LongContentDeserializer : PrimitiveContentDeserializer<long>
{
    /// <inheritdoc />
    protected override long DeserializeCore(string str)
    {
        return long.Parse(str);
    }
}