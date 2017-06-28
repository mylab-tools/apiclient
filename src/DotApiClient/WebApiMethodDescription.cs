using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DotApiClient
{
    partial class WebApiMethodDescription
    {
        public string MethodId { get; set; }

        public string RelPath { get; set; }

        public ContentType ContentType { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public WebApiParameterDescriptions Parameters { get; set; }

        public IEnumerable<WebApiMethodHeader> Headers { get; set; }

        public IEnumerable<string> UrlPartParameters { get; set; }
    }
    
    internal class WebApiMethodDescriptions : Dictionary<string, WebApiMethodDescription>
    {
        public WebApiMethodDescriptions()
        {

        }

        public WebApiMethodDescriptions(IEnumerable<WebApiMethodDescription> descriptions)
        {
            if (descriptions == null) throw new ArgumentNullException(nameof(descriptions));

            foreach (var webApiMethodDescription in descriptions)
            {
                Add(webApiMethodDescription);
            }
        }

        public void Add(WebApiMethodDescription apiMethodDescription)
        {
            Add(apiMethodDescription.MethodId, apiMethodDescription);
        }
    }

    struct WebApiMethodHeader
    {
        public string ParameterName { get; set; }
        public string HeaderName { get; set; }
    }
}