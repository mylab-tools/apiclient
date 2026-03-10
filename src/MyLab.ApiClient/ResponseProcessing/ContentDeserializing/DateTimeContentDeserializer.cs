using System;
using System.Globalization;

namespace MyLab.ApiClient.ResponseProcessing.ContentDeserializing;

class DateTimeContentDeserializer : PrimitiveContentDeserializer<DateTime>
{
    /// <inheritdoc />
    protected override DateTime DeserializeCore(string str)
    {
        return DateTime.Parse(str, CultureInfo.InvariantCulture);
    }
}