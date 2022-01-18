using Newtonsoft.Json;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Contains request factoring settings
    /// </summary>
    public class RequestFactoringSettings
    {
        /// <summary>
        /// JSON serialization settings
        /// </summary>
        public JsonSerializerSettings JsonSettings { get; set; }

        /// <summary>
        /// Creates req factoring settings from app options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static RequestFactoringSettings CreateFromOptions(ApiClientsOptions options)
        {
            var apiJsonSettings = options.JsonSettings ?? new ApiJsonSettings();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = apiJsonSettings.IgnoreNullValues
                    ? NullValueHandling.Ignore
                    : NullValueHandling.Include
            };

            return new RequestFactoringSettings
            {
                JsonSettings = jsonSerializerSettings
            };
        }
    }
}