using System.Collections.Generic;

namespace MyLab.ApiClient
{
    public class ApiClientsOptions
    {
        public Dictionary<string, ApiConnectionOptions> List { get; set; }
    }

    public class ApiConnectionOptions
    {
        public string Url { get; set; }
    }
}