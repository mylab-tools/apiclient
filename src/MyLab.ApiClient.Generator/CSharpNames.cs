using System.Globalization;
using System.Text;

namespace MyLab.ApiClient.Generator;

static class CSharpNames
{
    static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
        "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
        "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
        "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
        "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
        "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static",
        "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
        "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
    };

    public static string NamespaceFromTitle(string? title)
    {
        var identifier = ToPascal(title ?? string.Empty);
        return identifier.Length == 0 ? "Generated" : identifier;
    }

    public static string InterfaceName(string tag)
    {
        var pascal = ToPascal(tag);
        if (pascal.Length == 0)
            pascal = "Api";

        if (pascal.EndsWith("Api", StringComparison.Ordinal))
        {
            return pascal.Length > 1 && pascal[0] == 'I' && char.IsUpper(pascal[1])
                ? pascal
                : "I" + pascal;
        }

        return "I" + pascal + "Api";
    }

    public static string MethodName(string httpMethod, string path, string? operationId)
    {
        if (!string.IsNullOrWhiteSpace(operationId))
            return EscapeIdentifier(ToPascal(operationId));

        var builder = new StringBuilder(Verb(httpMethod));
        foreach (var segment in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment.StartsWith('{') && segment.EndsWith('}'))
            {
                builder.Append("By");
                builder.Append(ToPascal(segment.Trim('{', '}')));
            }
            else
            {
                builder.Append(ToPascal(segment));
            }
        }

        var name = builder.ToString();
        return name.Length == 0 ? "Call" : EscapeIdentifier(name);
    }

    public static string TypeName(string schemaName) => EscapeIdentifier(ToPascal(schemaName));

    public static string ParameterName(string wireName)
    {
        var camel = ToCamel(wireName);
        return camel.Length == 0 ? "value" : EscapeIdentifier(camel);
    }

    public static string ToPascal(string value)
    {
        var builder = new StringBuilder(value.Length);
        var startWord = true;

        foreach (var ch in value)
        {
            if (!char.IsLetterOrDigit(ch))
            {
                startWord = true;
                continue;
            }

            builder.Append(startWord ? char.ToUpper(ch, CultureInfo.InvariantCulture) : ch);
            startWord = false;
        }

        if (builder.Length == 0)
            return string.Empty;

        if (!char.IsLetter(builder[0]) && builder[0] != '_')
            builder.Insert(0, '_');

        return builder.ToString();
    }

    public static string ToCamel(string value)
    {
        var pascal = ToPascal(value);
        if (pascal.Length == 0)
            return pascal;

        return char.ToLower(pascal[0], CultureInfo.InvariantCulture) + pascal[1..];
    }

    public static string EscapeIdentifier(string identifier)
    {
        return Keywords.Contains(identifier) ? "@" + identifier : identifier;
    }

    public static string Literal(string value)
    {
        return "\"" + value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal) + "\"";
    }

    public static string Xml(string value)
    {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }

    static string Verb(string httpMethod) => httpMethod.ToUpperInvariant() switch
    {
        "GET" => "Get",
        "POST" => "Post",
        "PUT" => "Put",
        "PATCH" => "Patch",
        "DELETE" => "Delete",
        "HEAD" => "Head",
        _ => ToPascal(httpMethod.ToLowerInvariant())
    };
}
