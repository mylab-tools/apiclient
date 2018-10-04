using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyLab.ApiClient
{
    /// <summary>
    /// Represent simple code result
    /// </summary>
    public class CodeResult
    {
        /// <summary>
        /// Http status code
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Got message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CodeResult"/>
        /// </summary>
        public CodeResult(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        internal static async Task<CodeResult> CreateFromHttpMessage(HttpResponseMessage message)
        {
            var msg = await message.Content.ReadAsStringAsync();
            return new CodeResult(message.StatusCode, msg);
        }
    }
}
