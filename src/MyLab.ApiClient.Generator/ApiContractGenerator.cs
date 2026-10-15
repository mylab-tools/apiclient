using System.Text;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace MyLab.ApiClient.Generator;

/// <summary>
/// Generates MyLab.ApiClient contracts and data types from an OpenAPI 3 document.
/// </summary>
public static class ApiContractGenerator
{
    /// <summary>
    /// Generates C# sources from an OpenAPI 3 YAML document.
    /// </summary>
    public static GeneratedDocument Generate(string openApiYaml, GenerationOptions? options = null)
    {
        if (openApiYaml == null) throw new ArgumentNullException(nameof(openApiYaml));

        var document = Load(openApiYaml);
        var description = OpenApiDescriptionReader.Read(document, options);
        return new GeneratedDocument(description.Namespace, CSharpSourceEmitter.Emit(description));
    }

    static OpenApiDocument Load(string openApiYaml)
    {
        var settings = new OpenApiReaderSettings();
        settings.AddYamlReader();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(openApiYaml));
        var result = OpenApiDocument.Load(stream, OpenApiConstants.Yaml, settings);
        var diagnostic = result.Diagnostic;
        if (result.Document == null)
            throw new InvalidOperationException("OpenAPI document was not parsed. " + FormatDiagnostic(diagnostic));

        if (diagnostic?.SpecificationVersion == OpenApiSpecVersion.OpenApi2_0)
            throw new NotSupportedException("OpenAPI 2.0 documents are not supported.");

        if (diagnostic?.Errors is { Count: > 0 })
            throw new InvalidOperationException(FormatDiagnostic(diagnostic));

        return result.Document;
    }

    static string FormatDiagnostic(OpenApiDiagnostic? diagnostic)
    {
        if (diagnostic?.Errors == null || diagnostic.Errors.Count == 0)
            return "The reader did not return a document.";

        return string.Join(Environment.NewLine, diagnostic.Errors.Select(error => error.Message));
    }
}
