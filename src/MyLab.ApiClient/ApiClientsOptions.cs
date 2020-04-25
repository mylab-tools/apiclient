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
    }
}