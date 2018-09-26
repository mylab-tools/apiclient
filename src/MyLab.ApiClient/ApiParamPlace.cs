namespace MyLab.ApiClient
{
    /// <summary>
    /// Determines parameter place
    /// </summary>
    public enum ApiParamPlace
    {
        Undefined,
        /// <summary>
        /// Request body
        /// </summary>
        Body,
        /// <summary>
        /// Part of URL query
        /// </summary>
        Query,
        /// <summary>
        /// Part of URL path
        /// </summary>
        Path,
        /// <summary>
        /// HTTP header
        /// </summary>
        Header
    }
}