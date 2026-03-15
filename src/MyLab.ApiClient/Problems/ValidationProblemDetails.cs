using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyLab.ApiClient.Problems;

/// <summary>
/// A <see cref="ProblemDetails"/> for validation errors.
/// </summary>
public class ValidationProblemDetails : ProblemDetails
{

    /// <summary>
    /// Gets the validation errors associated with this instance of <see cref="ValidationProblemDetails"/>.
    /// </summary>
    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}