using System.Threading;

namespace MyLab.ApiClient.Usage.Invocation
{
    /// <summary>
    /// Contains indications of the source from which to obtain the CancellationToken.
    /// </summary>
    public static class CancellationTokenInsert
    {
        /// <summary>
        /// A source is Invocation method
        /// </summary>
        public static readonly CancellationToken FromInvoke = new();
    }
}
