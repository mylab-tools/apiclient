namespace DotApiClient
{
    /// <summary>
    /// Creates proxy for web api
    /// </summary>
    public interface IWebApiProxyFactory
    {
        /// <summary>
        /// Creates proxy 
        /// </summary>
        /// <typeparam name="T">web api contract</typeparam>
        T CreateProxy<T>() where T : class;
    }
}