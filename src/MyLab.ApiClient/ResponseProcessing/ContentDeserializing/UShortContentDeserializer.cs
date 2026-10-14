namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class UShortContentDeserializer : PrimitiveContentDeserializer<ushort>
{
    /// <inheritdoc />
    protected override ushort DeserializeCore(string str)
    {
        return ushort.Parse(str);
    }
}