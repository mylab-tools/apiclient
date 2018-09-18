namespace MyLab.ApiClient
{
    /// <summary>
    /// Web API client factory
    /// </summary>
    public class ApiClient<TContract>
    {
        /// <summary>
        /// Creates web api client by default
        /// </summary>
        public static TContract Create()
        {
            return new ApiClientBuilder<TContract>().Create();
        }
    }
}
