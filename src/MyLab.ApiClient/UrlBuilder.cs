using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLab.ApiClient
{
    class UrlBuilder
    {
        private readonly MethodDescription _methodDescription;
        private readonly string _contractRelPath;
        private readonly string _methodRelPath;

        private static readonly char[] TrimmedChars = {'\\', '/' };

        public static UrlBuilder GetForMethod(ApiClientDescription clientDescription, int metadataToken)
        {
            var methodDesc = clientDescription.GetMethod(metadataToken);
            return new UrlBuilder(clientDescription.RelPath, methodDesc);
        }

        public UrlBuilder(string clientRelPath, MethodDescription methodDescription)
        {
            _methodDescription = methodDescription ?? throw new ArgumentNullException(nameof(methodDescription));

            if (clientRelPath != null)
                _contractRelPath = clientRelPath.Trim(TrimmedChars);
            if (methodDescription.RelPath != null)
                _methodRelPath = methodDescription.RelPath.Trim(TrimmedChars);
        }

        public string Build(object[] args)
        {
            var b = new StringBuilder();

            AddPath(b);
            AddPathArgs(b, args);
            AddQueryArgs(b, args);

            return b.ToString();
        }

        private void AddPathArgs(StringBuilder b, object[] args)
        {
            foreach (var p in _methodDescription.Params)
            {
                if (p.Place != ApiParamPlace.Path) continue;

                b = b.Replace("{" + p.Name + "}", args[p.Position].ToString());
            }
        }

        private void AddQueryArgs(StringBuilder b, object[] args)
        {
            var queryArgs = _methodDescription.Params
                .Where(p => p.Place == ApiParamPlace.Query)
                .Select((d, i) => d.Name + "=" + args[i])
                .ToArray();

            if (queryArgs.Length != 0)
                b.Append("?" + string.Join("&", queryArgs));
        }

        private void AddPath(StringBuilder b)
        {
            if (_contractRelPath != null)
                b.Append(_contractRelPath);
            if (_methodRelPath != null)
            {
                if (b.Length != 0)
                    b.Append("/" + _methodRelPath);
                else
                    b.Append(_methodRelPath);
            }
        }
    }
}