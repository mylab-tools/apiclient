using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    class MediaTypeProc
    {
        private readonly HttpContent _content;
        private IDictionary<string, Func<HttpContent, Task<object>>> _creators = new Dictionary<string,Func<HttpContent, Task<object>>>();
        private Func<HttpContent, Task<object>> _defaultCreator;


        public MediaTypeProc(HttpContent content)
        {
            _content = content;
        }

        public MediaTypeProc Supports(string mimeType, Func<HttpContent, Task<object>> creator)
        {
            var newCreatorMap = new Dictionary<string, Func<HttpContent, Task<object>>>(_creators)
            {
                {mimeType, creator}
            };

            return new MediaTypeProc(_content)
            {
                _creators = newCreatorMap,
                _defaultCreator = _defaultCreator
            };
        }

        public MediaTypeProc Default(Func<HttpContent, Task<object>> creator)
        {
            return new MediaTypeProc(_content)
            {
                _creators = _creators,
                _defaultCreator = creator
            };
        }

        public async Task<object> GetResult()
        {
            if(_creators.TryGetValue(_content.Headers.ContentType.MediaType, out var creator))
            {
                return await creator(_content);
            }

            if (_defaultCreator != null)
            {
                return await _defaultCreator(_content);
            }

            throw new UnexpectedResponseContentTypeException(_content.Headers.ContentType.MediaType);
        }
    }
}