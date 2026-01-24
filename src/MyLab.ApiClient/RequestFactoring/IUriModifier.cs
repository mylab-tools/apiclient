using System;
using System.Web;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.RequestFactoring
{
    /// <summary>
    /// Modifies a URI
    /// </summary>
    public interface IUriModifier
    {
        /// <summary>
        /// Modifies a URI
        /// </summary>
        Uri Modify(Uri origin, string paramName, object? value);
    }

    class UrlPathInjector : IUriModifier
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

    class UrlQueryInjector : IUriModifier
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
}
