using System;
using System.Web;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.RequestFactoring.UrlModifying;

class UrlPathInjector : IUrlModifier
{
    public Uri Modify(Uri origin, string paramName, object? value)
    {
        if (origin == null) throw new ArgumentNullException(nameof(origin));
        if (string.IsNullOrEmpty(paramName))
            throw new ArgumentException("Value cannot be null or empty.", nameof(paramName));


        var strVal = value != null 
            ? ObjectToStringConverter.ToString(value)
            : string.Empty;
            
        var tag = "{" + paramName + "}";

        var decodedPath = HttpUtility.UrlDecode(origin.IsAbsoluteUri
            ? origin.PathAndQuery
            : origin.OriginalString);
        string resultPath;

        if (!string.IsNullOrWhiteSpace(strVal))
        {
            resultPath = decodedPath.Replace(tag, Uri.EscapeDataString(strVal));
        }
        else
        {
            resultPath = decodedPath
                .Replace($"{tag}/", string.Empty)
                .Replace($"{tag}", string.Empty);
        }

        if (origin.IsAbsoluteUri)
        {
            var b = new UriBuilder(origin.Scheme, origin.Host, origin.Port, resultPath);

            return b.Uri;
        }
        else
        {
            return new Uri(resultPath, UriKind.Relative);
        }
    }
}