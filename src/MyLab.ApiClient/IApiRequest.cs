namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides ability to call web api service
    /// </summary>
    public interface IApiRequest
    {
        void Call();
    }
    
    /// <summary>
    /// Provides ability to call web api service
    /// </summary>
    public interface IApiRequest<TRes>
    {
        TRes Call();
    }
}