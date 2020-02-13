using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyLab.ApiClient
{
    class RequestParametersDescriptions
    {
        public IReadOnlyList<UrlRequestParameterDescription> UrlParams { get; }
        public IReadOnlyList<HeaderRequestParameterDescription> HeaderParams { get; }
        public IReadOnlyList<ContentRequestParameterDescription> ContentParams { get; }

        public RequestParametersDescriptions(
            IEnumerable<UrlRequestParameterDescription> urlParams,
            IEnumerable<HeaderRequestParameterDescription> headerParams,
            IEnumerable<ContentRequestParameterDescription> contentParams)
        {
            UrlParams = new List<UrlRequestParameterDescription>(urlParams);
            HeaderParams = new List<HeaderRequestParameterDescription>(headerParams);
            ContentParams = new List<ContentRequestParameterDescription>(contentParams);
        }

        public static RequestParametersDescriptions Create(MethodInfo mi)
        {
            var urlParams = new List<UrlRequestParameterDescription>();
            var headerParams = new List<HeaderRequestParameterDescription>();
            var contentParams = new List<ContentRequestParameterDescription>();

            var props = mi.GetParameters();

            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];

                var attr = GetParamAttr(p);

                if (attr is UrlParameterAttribute urlA)
                    urlParams.Add(new UrlRequestParameterDescription(i, urlA.Name ?? p.Name, urlA.UrlModifier));
                else if (attr is HeaderParameterAttribute hdrA)
                    headerParams.Add(new HeaderRequestParameterDescription(i, hdrA.Name ?? p.Name));
                else if (attr is ContentParameterAttribute contA)
                    contentParams.Add(new ContentRequestParameterDescription(i, contA.HttpContentFactory));
                else
                    throw new ApiClientException($"Request attribute type '{attr.GetType().FullName}' not supported");
            }

            return new RequestParametersDescriptions(
                urlParams,
                headerParams,
                contentParams);
        }

        static ApiParameterAttribute GetParamAttr(ParameterInfo p)
        {
            var pas = p.GetCustomAttributes<ApiParameterAttribute>().ToArray();
            if (pas.Length == 0)
                throw new ApiContractException($"An API method parameter must be marked with one of inheritors of '{typeof(ApiParameterAttribute)}'");
            if (pas.Length > 1)
                throw new ApiContractException($"Only single '{typeof(ApiParameterAttribute)}' attribute is supported");

            return pas.Single();
        }
    }

    interface IRequestParameterDescription
    {
        int Position { get; }
    }

    class UrlRequestParameterDescription : IRequestParameterDescription
    {
        public int Position { get; }
        public string Name { get; }
        public IUriModifier Modifier { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UrlRequestParameterDescription"/>
        /// </summary>
        public UrlRequestParameterDescription(int position, string name, IUriModifier modifier)
        {
            Position = position;
            Name = name;
            Modifier = modifier;
        }
    }

    class HeaderRequestParameterDescription : IRequestParameterDescription
    {
        public int Position { get; }
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HeaderRequestParameterDescription"/>
        /// </summary>
        public HeaderRequestParameterDescription(int position, string name)
        {
            Position = position;
            Name = name;
        }
    }
    class ContentRequestParameterDescription : IRequestParameterDescription
    {
        public int Position { get; }
        public IHttpContentFactory ContentFactory { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ContentRequestParameterDescription"/>
        /// </summary>
        public ContentRequestParameterDescription(int position, IHttpContentFactory contentFactory)
        {
            Position = position;
            ContentFactory = contentFactory;
        }
    }
}
