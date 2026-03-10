using System.Globalization;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class FloatContentDeserializer : PrimitiveContentDeserializer<float>
{
    /// <inheritdoc />
    protected override float DeserializeCore(string str)
    {
        return float.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
    }
}