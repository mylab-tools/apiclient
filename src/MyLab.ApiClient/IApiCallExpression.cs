namespace MyLab.ApiClient
{
    /// <summary>
    /// Provides ability to call web api service
    /// </summary>
    public interface IApiCallExpression
    {
        void Call();
    }
    
    /// <summary>
    /// Provides ability to call web api service
    /// </summary>
    public interface IApiCallExpression<TRes>
    {
        TRes Call();
    }
}