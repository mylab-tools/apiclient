using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using MyLab.ApiClient.Contracts.Attributes.ForData;

namespace MyLab.ApiClient.RequestFactoring.ContentFactoring;

class UrlFormHttpContentFactory : IHttpContentFactory
{
    public HttpContent Create(object? source, RequestFactoringSettings? settings)
    {
        string content = string.Empty;

        if (source != null)
        {
            var queryBuilder = new StringBuilder();
            var props = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in props)
            {
                var formItemAttr = prop.GetCustomAttribute<UrlFormItemAttribute>();

                string name = formItemAttr?.Name ?? prop.Name;

                var strVal = prop.GetValue(source)?.ToString();

                if (strVal != null)
                {
                    var normVal = settings is { UrlFormSettings: { EscapeSymbols: true } }
                        ? Uri.EscapeDataString(strVal)
                        : strVal;

                    queryBuilder.Append($"&{name}={normVal}");
                }
                else
                {
                    if (settings is { UrlFormSettings: { IgnoreNullValues: false } })
                    {
                        queryBuilder.Append($"&{name}=");
                    }
                }
            }

            content = queryBuilder.ToString().TrimStart('&');
        }

        var httpContent = new StringContent(content);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        return httpContent;
    }
}