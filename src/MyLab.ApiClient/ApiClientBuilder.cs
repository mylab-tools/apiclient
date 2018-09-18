using System;
using System.Collections.Generic;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Web API client builder
    /// </summary>
    public class ApiClientBuilder<TContract>
    {
        readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public ApiClientBuilder<TContract> AddHeader(string name, string value)
        {
            _headers.Add(name, value);

            return this;
        }

        public TContract Create()
        {
            throw new NotImplementedException();
        }
    }
}