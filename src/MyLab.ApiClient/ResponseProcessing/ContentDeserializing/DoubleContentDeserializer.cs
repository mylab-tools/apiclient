using System.Globalization;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class DoubleContentDeserializer : PrimitiveContentDeserializer<double>
{
    /// <inheritdoc />
    protected override double DeserializeCore(string str)
    {
        return double.Parse(str.Replace(",", "."), CultureInfo.InvariantCulture);
    }
}