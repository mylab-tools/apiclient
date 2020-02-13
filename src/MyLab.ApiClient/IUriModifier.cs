
using System;
using System.Web;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Modifies a URI
    /// </summary>
    public interface IUriModifier
    {
        /// <summary>
        /// Modifies a URI
        /// </summary>
        Uri Modify(Uri origin, string paramName, object value);
    }
    
    class UrlPathInjector : IUriModifier
    {
        public Uri Modify(Uri origin, string paramName, object value)
        { 
            var strVal = value?.ToString() ?? string.Empty;
            var tag = "{" + paramName + "}";

            var b =new UriBuilder(origin);
            var decodedPath = HttpUtility.UrlDecode(b.Path);

            if (!string.IsNullOrWhiteSpace(strVal))
            {
                b.Path = decodedPath.Replace(tag, strVal);
            }
            else
            {
                b.Path = decodedPath
                    .Replace($"{tag}/", string.Empty)
                    .Replace($"{tag}", string.Empty);
            }

            return b.Uri;
        }
    }

    class UrlQueryInjector : IUriModifier
    {
        public Uri Modify(Uri origin, string paramName, object value)
        {
            var strVal = value?.ToString() ?? string.Empty;

            var b = new UriBuilder(origin);
            var querySeparator = string.IsNullOrEmpty(b.Query) ? string.Empty : "&";

            b.Query += $"{querySeparator}{paramName}={strVal}";

            return b.Uri;
        }
    }
}
