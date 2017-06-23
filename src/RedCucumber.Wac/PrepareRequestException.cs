namespace RedCucumber.Wac
{
    /// <summary>
    /// Throws when request preparing error occured
    /// </summary>
    public class PrepareRequestException : WebApiException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PrepareRequestException"/>
        /// </summary>
        public PrepareRequestException()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="PrepareRequestException"/>
        /// </summary>
        public PrepareRequestException(string message)
            :base(message)
        {
            
        }
    }
}