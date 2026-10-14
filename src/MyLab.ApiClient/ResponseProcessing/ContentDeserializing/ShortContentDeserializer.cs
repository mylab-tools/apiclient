namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class ShortContentDeserializer : PrimitiveContentDeserializer<short>
{
    /// <inheritdoc />
    protected override short DeserializeCore(string str)
    {
        return short.Parse(str);
    }
}