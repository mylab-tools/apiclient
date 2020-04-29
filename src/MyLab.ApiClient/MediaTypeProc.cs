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
                _creators = newCreatorMap
            };
        }

        public async Task<object> GetResult()
        {
            if(_creators.TryGetValue(_content.Headers.ContentType.MediaType, out var creator))
            {
                return await creator(_content);
            }

            throw new UnexpectedResponseContentTypeException(_content.Headers.ContentType.MediaType);
        }
    }
}