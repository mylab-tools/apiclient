using System.Collections.Generic;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains api clients infrastructure options
    /// </summary>
    public class ApiClientsOptions
    {
        /// <summary>
        /// List of api connections options
        /// </summary>
        public Dictionary<string, ApiConnectionOptions> List { get; set; }

        /// <summary>
        /// Defines JSON serialization settings
        /// </summary>
        public ApiJsonSettings JsonSettings { get; set; }
    }

    /// <summary>
    /// Contains api connection options
    /// </summary>
    public class ApiConnectionOptions
    {
        /// <summary>
        /// API base url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Do not verify the server SSL certificate
        /// </summary>
        public bool SkipServerSslCertVerification { get; set; }
    }

    /// <summary>
    /// Defines JSON serialization settings
    /// </summary>
    public class ApiJsonSettings
    {
        /// <summary>
        /// Ignores null fields. True by default.
        /// </summary>
        public bool IgnoreNullValues { get; set; } = true;
    }
}