using System;
using MyLab.ApiClient.Tools;

namespace MyLab.ApiClient.RequestFactoring.UrlModifying;

class UrlPathInjector : IUrlModifier
{
    public Uri Modify(Uri origin, string paramName, object? value)
    {
        if (origin == null) throw new ArgumentNullException(nameof(origin));
        if (string.IsNullOrEmpty(paramName))
            throw new ArgumentException("Value cannot be null or empty.", nameof(paramName));

        var strVal = ObjectToStringConverter.ToString(value);
        var tag = "{" + paramName + "}";

        SplitPathAndQuery(origin, out var path, out var query);

        string resultPath;
        if (!string.IsNullOrWhiteSpace(strVal))
        {
            resultPath = path.Replace(tag, EscapePathSegment(strVal));
        }
        else
        {
            resultPath = path
                .Replace($"{tag}/", string.Empty)
                .Replace(tag, string.Empty);
        }

        var querySuffix = string.IsNullOrEmpty(query) ? string.Empty : "?" + query;

        if (origin.IsAbsoluteUri)
        {
            return new Uri(GetAbsolutePrefix(origin) + resultPath + querySuffix, UriKind.Absolute);
        }

        return new Uri(resultPath + querySuffix, UriKind.Relative);
    }

    /// <summary>
    /// Encodes a single path segment. Dot segments ("." / "..") are encoded explicitly
    /// because <see cref="Uri.EscapeDataString(string)"/> leaves '.' unescaped (RFC 3986 unreserved).
    /// </summary>
    static string EscapePathSegment(string value)
    {
        var escaped = Uri.EscapeDataString(value);

        if (escaped is "." or "..")
            return escaped.Replace(".", "%2E");

        return escaped;
    }

    /// <summary>
    /// Splits path and query from <see cref="Uri.OriginalString"/> without decoding,
    /// so encoded template segments like %2E%2E / %2F stay intact.
    /// </summary>
    static void SplitPathAndQuery(Uri origin, out string path, out string query)
    {
        var original = StripFragment(origin.OriginalString);

        if (origin.IsAbsoluteUri)
        {
            var authorityStart = GetAuthorityStart(original);
            var pathStart = original.IndexOf('/', authorityStart);
            if (pathStart < 0)
            {
                path = "/";
                query = string.Empty;
                return;
            }

            SplitAtQuery(original, pathStart, out path, out query);
        }
        else
        {
            SplitAtQuery(original, 0, out path, out query);
        }
    }

    static void SplitAtQuery(string value, int startIndex, out string path, out string query)
    {
        var queryStart = value.IndexOf('?', startIndex);
        if (queryStart < 0)
        {
            path = value.Substring(startIndex);
            query = string.Empty;
        }
        else
        {
            path = value.Substring(startIndex, queryStart - startIndex);
            query = value.Substring(queryStart + 1);
        }
    }

    static string GetAbsolutePrefix(Uri origin)
    {
        var original = StripFragment(origin.OriginalString);
        var authorityStart = GetAuthorityStart(original);
        var pathStart = original.IndexOf('/', authorityStart);

        return pathStart < 0
            ? original
            : original.Substring(0, pathStart);
    }

    static int GetAuthorityStart(string absoluteUri)
    {
        var schemeSep = absoluteUri.IndexOf("://", StringComparison.Ordinal);
        return schemeSep >= 0 ? schemeSep + 3 : 0;
    }

    static string StripFragment(string uriString)
    {
        var fragmentStart = uriString.IndexOf('#');
        return fragmentStart < 0 ? uriString : uriString.Substring(0, fragmentStart);
    }
}
