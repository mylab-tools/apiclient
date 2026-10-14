using System.Globalization;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class DecimalContentDeserializer : PrimitiveContentDeserializer<decimal>
{
    /// <inheritdoc />
    protected override decimal DeserializeCore(string str)
    {
        return decimal.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
    }
}