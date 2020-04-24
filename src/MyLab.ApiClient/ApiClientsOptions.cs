using System.Collections.Generic;

namespace MyLab.ApiClient
{
    public class ApiClientsOptions
    {
        public Dictionary<string, ApiClientOptionsDescription> List { get; set; }
    }

    public class ApiClientOptionsDescription
    {
        public string Url { get; set; }
    }
}