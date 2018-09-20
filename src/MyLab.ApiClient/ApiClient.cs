namespace MyLab.ApiClient
{
    /// <summary>
    /// Web API client factory
    /// </summary>
    public class ApiClient<TContract>
        where TContract : class 
    {
        /// <summary>
        /// Creates web api client by default
        /// </summary>
        public static TContract Create(string basePath)
        {
            return new ApiClientBuilder<TContract>
            {
                BasePath = basePath
            }.Create();
        }
    }
}
