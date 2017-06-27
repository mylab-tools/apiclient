using System.Collections.Generic;

namespace DotAspectClient
{
    /// <summary>
    /// Contians web api interaction options
    /// </summary>
    public class WebApiClientOptions
    {
        /// <summary>
        /// Gets non-default request processor
        /// </summary>
        public IWebApiRequestProcessor RequestProcessor { get; set; }

        /// <summary>
        /// Gets predefined request headers
        /// </summary>
        public IEnumerable<RequestHeader> PredefinedHeaders { get; set; }
    }

    /// <summary>
    /// Dscribe request message header
    /// </summary>
    public class RequestHeader
    {
        /// <summary>
        /// Header name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Header value
        /// </summary>
        public string Value { get; set; }
    }
}