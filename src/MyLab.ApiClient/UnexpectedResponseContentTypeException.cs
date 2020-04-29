namespace MyLab.ApiClient
{
    /// <summary>
    /// Throws when response content has unexpected media type
    /// </summary>
    public class UnexpectedResponseContentTypeException : ApiClientException
    {
        public string ActualMediaType { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UnexpectedResponseContentTypeException"/>
        /// </summary>
        public UnexpectedResponseContentTypeException(string actualMediaType)
            :base($"The response has unexpected media type: '{actualMediaType}'")
        {
            ActualMediaType = actualMediaType;
        }
    }
}