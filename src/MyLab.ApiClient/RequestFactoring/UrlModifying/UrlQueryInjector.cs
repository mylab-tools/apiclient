using System;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.RequestFactoring.UrlModifying;

class UrlQueryInjector : IUrlModifier
{
    public Uri Modify(Uri origin, string paramName, object? value)
    {
        if (origin == null) throw new ArgumentNullException(nameof(origin));
        if (string.IsNullOrEmpty(paramName))
            throw new ArgumentException("Value cannot be null or empty.", nameof(paramName));
            
        var strVal = value != null 
            ? Uri.EscapeDataString(ObjectToStringConverter.ToString(value))
            : string.Empty;

        var uriStr = origin.OriginalString;
        var queryStart = uriStr.IndexOf("?", StringComparison.InvariantCulture);

        var originQuery = queryStart != -1
            ? uriStr.Substring(queryStart + 1)
            : string.Empty;

        var querySeparator = string.IsNullOrEmpty(originQuery) ? string.Empty : "&";
        var query = originQuery + $"{querySeparator}{paramName}={strVal}";


        var uriWithoutQuery = queryStart >= 0 
            ? uriStr.Remove(queryStart) 
            : uriStr;

        var queryBegin = string.IsNullOrWhiteSpace(query) ? string.Empty : "?";
        return new Uri($"{uriWithoutQuery}{queryBegin}{query}", UriKind.RelativeOrAbsolute);
    }
}