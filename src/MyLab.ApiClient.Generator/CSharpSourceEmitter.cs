using System.Text;

namespace MyLab.ApiClient.Generator;

static class CSharpSourceEmitter
{
    public static IReadOnlyList<GeneratedFile> Emit(ApiDescription description)
    {
        var files = new List<GeneratedFile>();

        foreach (var schema in description.Schemas)
            files.Add(new GeneratedFile(schema.Name + ".cs", EmitSchema(description.Namespace, schema)));

        foreach (var contract in description.Contracts)
            files.Add(new GeneratedFile(contract.Name + ".cs", EmitContract(description, contract)));

        return files;
    }

    static string EmitSchema(string ns, DataDescription schema)
    {
        var code = new CodeBuilder();
        code.Line("#nullable enable");
        if (NeedsSystem(schema))
            code.Line("using System;");
        if (schema.Properties.Any(property => property.TypeName.Contains("List<", StringComparison.Ordinal)))
            code.Line("using System.Collections.Generic;");
        code.Line("using System.Text.Json.Serialization;");
        code.Line("using Newtonsoft.Json;");
        code.Line();
        code.Line("namespace " + ns + ";");
        code.Line();
        WriteSummary(code, schema.Description);
        code.Line("public class " + schema.Name);
        code.Open("{");

        for (var i = 0; i < schema.Properties.Count; i++)
        {
            if (i > 0)
                code.Line();

            var property = schema.Properties[i];
            WriteSummary(code, property.Description);
            code.Line("[JsonPropertyName(" + CSharpNames.Literal(property.JsonName) + ")]");
            code.Line("[JsonProperty(" + CSharpNames.Literal(property.JsonName) + ")]");

            var typeName = property.Optional ? property.TypeName + "?" : property.TypeName;
            var initializer = property.Optional || !property.IsReferenceType ? string.Empty : " = null!;";
            code.Line($"public {typeName} {property.ClrName} {{ get; set; }}{initializer}");
        }

        code.Close();
        return code.ToString();
    }

    static string EmitContract(ApiDescription description, ContractDescription contract)
    {
        var code = new CodeBuilder();
        code.Line("#nullable enable");
        if (contract.Operations.Any(operation => operation.Deprecated))
            code.Line("using System;");
        code.Line("using System.Threading.Tasks;");
        code.Line("using MyLab.ApiClient.Contracts.Attributes.ForContract;");
        code.Line("using MyLab.ApiClient.Contracts.Attributes.ForMethod;");
        code.Line("using MyLab.ApiClient.Contracts.Attributes.ForParameters;");
        code.Line();
        code.Line("namespace " + description.Namespace + ";");
        code.Line();
        WriteSummary(code, contract.Summary);
        WriteServerRemarks(code, description);
        if (description.Binding == null)
            code.Line("[ApiContract]");
        else
            code.Line("[ApiContract(Binding = " + CSharpNames.Literal(description.Binding) + ")]");

        code.Line("public interface " + contract.Name);
        code.Open("{");

        for (var i = 0; i < contract.Operations.Count; i++)
        {
            if (i > 0)
                code.Line();

            WriteOperation(code, contract.Operations[i]);
        }

        code.Close();
        return code.ToString();
    }

    static void WriteOperation(CodeBuilder code, OperationDescription operation)
    {
        WriteSummary(code, operation.Summary);
        WriteRemarks(code, operation.Remarks);
        foreach (var parameter in operation.Parameters)
        {
            if (string.IsNullOrWhiteSpace(parameter.Description))
                continue;

            code.Line("/// <param name=\"" + parameter.Name + "\">" + CSharpNames.Xml(OneLine(parameter.Description)) + "</param>");
        }

        if (!string.IsNullOrWhiteSpace(operation.Returns))
            code.Line("/// <returns>" + CSharpNames.Xml(OneLine(operation.Returns)) + "</returns>");

        if (operation.Deprecated)
            code.Line("[Obsolete]");

        var attribute = operation.Url.Length == 0
            ? "[" + operation.HttpAttribute + "]"
            : "[" + operation.HttpAttribute + "(" + CSharpNames.Literal(operation.Url) + ")]";
        code.Line(attribute);

        var signature = new StringBuilder();
        signature.Append(operation.ReturnType);
        signature.Append(' ');
        signature.Append(operation.Name);
        signature.Append('(');
        signature.Append(string.Join(", ", operation.Parameters.Select(FormatParameter)));
        signature.Append(");");
        code.Line(signature.ToString());
    }

    static string FormatParameter(ParameterDescription parameter)
    {
        var attribute = parameter.AttributeArgument == null
            ? "[" + parameter.Attribute + "]"
            : "[" + parameter.Attribute + "(" + CSharpNames.Literal(parameter.AttributeArgument) + ")]";
        var typeName = parameter.Optional ? parameter.TypeName + "?" : parameter.TypeName;
        var suffix = parameter.Optional ? " = null" : string.Empty;
        return attribute + " " + typeName + " " + parameter.Name + suffix;
    }

    static void WriteServerRemarks(CodeBuilder code, ApiDescription description)
    {
        if (description.Servers.Count == 0 && description.Version == null)
            return;

        code.Line("/// <remarks>");
        if (!string.IsNullOrWhiteSpace(description.Version))
            code.Line("/// Version " + CSharpNames.Xml(description.Version));

        foreach (var server in description.Servers)
        {
            var text = string.IsNullOrWhiteSpace(server.Description)
                ? server.Url
                : server.Url + " — " + server.Description;
            code.Line("/// " + CSharpNames.Xml(OneLine(text)));
        }

        code.Line("/// </remarks>");
    }

    static void WriteSummary(CodeBuilder code, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        code.Line("/// <summary>");
        foreach (var line in text.Split('\n'))
        {
            var trimmed = line.Trim('\r').Trim();
            if (trimmed.Length > 0)
                code.Line("/// " + CSharpNames.Xml(trimmed));
        }

        code.Line("/// </summary>");
    }

    static void WriteRemarks(CodeBuilder code, string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        code.Line("/// <remarks>");
        foreach (var line in text.Split('\n'))
        {
            var trimmed = line.Trim('\r').Trim();
            if (trimmed.Length > 0)
                code.Line("/// " + CSharpNames.Xml(trimmed));
        }

        code.Line("/// </remarks>");
    }

    static bool NeedsSystem(DataDescription schema)
    {
        return schema.Properties.Any(property =>
            property.TypeName.Contains("DateOnly", StringComparison.Ordinal) ||
            property.TypeName.Contains("DateTime", StringComparison.Ordinal) ||
            property.TypeName.Contains("Guid", StringComparison.Ordinal) ||
            property.TypeName.Contains("TimeSpan", StringComparison.Ordinal));
    }

    static string OneLine(string value) => value.Replace("\r", " ", StringComparison.Ordinal)
        .Replace("\n", " ", StringComparison.Ordinal)
        .Trim();

    sealed class CodeBuilder
    {
        readonly StringBuilder _builder = new();
        int _indent;

        public void Line(string text = "")
        {
            if (text.Length == 0)
            {
                _builder.AppendLine();
                return;
            }

            _builder.Append(' ', _indent * 4);
            _builder.AppendLine(text);
        }

        public void Open(string text)
        {
            Line(text);
            _indent++;
        }

        public void Close(string text = "}")
        {
            _indent--;
            Line(text);
        }

        public override string ToString() => _builder.ToString();
    }
}
