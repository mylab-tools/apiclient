namespace MyLab.ApiClient.Options;

/// <summary>
/// Contains api connection options
/// </summary>
public class ApiEndpointOptions
{
    /// <summary>
    /// API base url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Do not verify the server SSL certificate
    /// </summary>
    public bool SkipServerSslCertVerification { get; set; }
}