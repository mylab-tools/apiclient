namespace RedCucumber.Wac
{
    /// <summary>
    /// Determines file for web api request
    /// </summary>
    public class WebApiFile
    {
        /// <summary>
        /// Filename
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Binary content
        /// </summary>
        public byte[] Content { get; set; }
    }
}
