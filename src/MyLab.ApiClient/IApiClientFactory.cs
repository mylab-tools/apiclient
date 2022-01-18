namespace MyLab.ApiClient
{
    /// <summary>
    /// Creates Api clients
    /// </summary>
    public interface IApiClientFactory
    {
        /// <summary>
        /// Creates api client for specified contract
        /// </summary>
        ApiClient<TContract> CreateApiClient<TContract>();
    }
}