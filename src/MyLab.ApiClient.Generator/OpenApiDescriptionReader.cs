using Microsoft.OpenApi;

namespace MyLab.ApiClient.Generator;

sealed class OpenApiDescriptionReader
{
    readonly Dictionary<string, DataDescription> _models = new(StringComparer.Ordinal);
    readonly Dictionary<string, TypeUse> _aliases = new(StringComparer.Ordinal);
    readonly HashSet<string> _modelNames = new(StringComparer.Ordinal);

    public static ApiDescription Read(OpenApiDocument document, GenerationOptions? options)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        return new OpenApiDescriptionReader().ReadDocument(document, options);
    }

    ApiDescription ReadDocument(OpenApiDocument document, GenerationOptions? options)
    {
        var title = document.Info?.Title;
        var description = new ApiDescription
        {
            Namespace = string.IsNullOrWhiteSpace(options?.Namespace)
                ? CSharpNames.NamespaceFromTitle(title)
                : options!.Namespace!,
            Binding = string.IsNullOrWhiteSpace(title) ? null : title,
            Version = string.IsNullOrWhiteSpace(document.Info?.Version) ? null : document.Info!.Version
        };

        if (document.Servers != null)
        {
            foreach (var server in document.Servers)
            {
                if (string.IsNullOrWhiteSpace(server.Url))
                    continue;

                description.Servers.Add(new ServerDescription
                {
                    Url = server.Url,
                    Description = server.Description
                });
            }
        }

        ReadSchemas(document);

        foreach (var contract in ReadContracts(document, title))
            description.Contracts.Add(contract);

        description.Schemas.AddRange(_models.Values.OrderBy(model => model.Name, StringComparer.Ordinal));
        return description;
    }

    void ReadSchemas(OpenApiDocument document)
    {
        if (document.Components?.Schemas is not { Count: > 0 } schemas)
            return;

        foreach (var (name, schema) in schemas)
        {
            var concrete = Unwrap(schema, name);
            RejectComposition(concrete, name);

            if (IsObject(concrete))
            {
                var model = new DataDescription
                {
                    Name = ReserveTypeName(name),
                    Description = concrete.Description
                };
                _models.Add(name, model);
            }
        }

        foreach (var (name, schema) in schemas)
        {
            var concrete = Unwrap(schema, name);
            if (_models.TryGetValue(name, out var model))
                FillProperties(model, concrete);
            else
                _aliases[name] = MapValue(concrete, name);
        }
    }

    List<ContractDescription> ReadContracts(OpenApiDocument document, string? title)
    {
        var groups = new Dictionary<string, ContractDescription>(StringComparer.Ordinal);
        var interfaceNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var (path, pathItem) in document.Paths)
        {
            if (pathItem.Operations is not { Count: > 0 } operations)
                continue;

            foreach (var (httpMethod, operation) in operations)
            {
                var tag = operation.Tags?.Select(item => item.Name).FirstOrDefault(name => !string.IsNullOrWhiteSpace(name))
                          ?? title
                          ?? "Api";
                if (!groups.TryGetValue(tag, out var contract))
                {
                    contract = new ContractDescription
                    {
                        Name = ReserveInterfaceName(tag, interfaceNames),
                        Summary = tag
                    };
                    groups.Add(tag, contract);
                }

                contract.Operations.Add(ReadOperation(path, pathItem, httpMethod.Method, operation, contract));
            }
        }

        return groups.Values.ToList();
    }

    OperationDescription ReadOperation(
        string path,
        IOpenApiPathItem pathItem,
        string httpMethod,
        OpenApiOperation operation,
        ContractDescription contract)
    {
        var url = path.Trim('/');
        var methodName = Unique(
            CSharpNames.MethodName(httpMethod, path, operation.OperationId),
            contract.Operations.Select(item => item.Name));

        var description = new OperationDescription
        {
            Name = methodName,
            HttpAttribute = HttpAttribute(httpMethod),
            Url = url,
            ReturnType = "Task",
            Summary = string.IsNullOrWhiteSpace(operation.Summary) ? null : operation.Summary.Trim(),
            Remarks = string.IsNullOrWhiteSpace(operation.Description) ? null : operation.Description.Trim(),
            Deprecated = operation.Deprecated
        };

        if (description.Remarks == description.Summary)
            description.Remarks = null;

        var usedNames = new HashSet<string>(StringComparer.Ordinal);
        var required = new List<ParameterDescription>();
        var optional = new List<ParameterDescription>();

        foreach (var parameter in MergeParameters(path, pathItem, operation))
            (IsRequired(parameter) ? required : optional).Add(ReadParameter(parameter, methodName, usedNames));

        if (operation.RequestBody != null)
        {
            var body = ReadBody(operation.RequestBody, methodName, usedNames);
            (body.Optional ? optional : required).Add(body);
        }

        description.Parameters.AddRange(required);
        description.Parameters.AddRange(optional);

        ApplyResponse(description, operation);
        return description;
    }

    void ApplyResponse(OperationDescription description, OpenApiOperation operation)
    {
        var response = SelectSuccess(operation.Responses);
        if (response == null)
            return;

        description.Returns = string.IsNullOrWhiteSpace(response.Description) ? null : response.Description.Trim();

        var media = Pick(response.Content);
        if (media == null)
            return;

        var type = TypeForMedia(media.Value.MediaType, media.Value.Media.Schema, description.Name + "Response");
        var nullable = media.Value.Media.Schema != null && IsNullable(media.Value.Media.Schema);
        var typeName = nullable ? type.Name + "?" : type.Name;
        description.ReturnType = "Task<" + typeName + ">";
    }

    ParameterDescription ReadParameter(IOpenApiParameter parameter, string methodName, HashSet<string> usedNames)
    {
        var schema = parameter.Schema ?? throw new InvalidOperationException(
            $"Parameter '{parameter.Name}' does not define a schema.");
        var type = Resolve(schema, methodName + CSharpNames.ToPascal(parameter.Name ?? "value"));
        var optional = !IsRequired(parameter) || IsNullable(schema);

        return new ParameterDescription
        {
            Name = Unique(CSharpNames.ParameterName(parameter.Name ?? "value"), usedNames),
            Attribute = AttributeName(parameter.In),
            AttributeArgument = parameter.Name,
            TypeName = type.Name,
            Optional = optional,
            Description = parameter.Description ?? schema.Description
        };
    }

    ParameterDescription ReadBody(IOpenApiRequestBody requestBody, string methodName, HashSet<string> usedNames)
    {
        var body = Unwrap(requestBody);
        var media = Pick(body.Content) ?? throw new InvalidOperationException(
            $"Operation '{methodName}' request body does not define a supported media type.");
        var schema = media.Media.Schema;
        var type = TypeForMedia(media.MediaType, schema, methodName + "Body");
        var referenceName = schema is OpenApiSchemaReference reference ? reference.Reference.Id : null;
        var parameterName = string.IsNullOrWhiteSpace(referenceName) ? "body" : CSharpNames.ToCamel(referenceName);
        var optional = !body.Required || (schema != null && IsNullable(schema));

        return new ParameterDescription
        {
            Name = Unique(CSharpNames.EscapeIdentifier(parameterName), usedNames),
            Attribute = ContentAttribute(media.MediaType),
            TypeName = type.Name,
            Optional = optional,
            Description = body.Description ?? schema?.Description
        };
    }

    TypeUse TypeForMedia(string mediaType, IOpenApiSchema? schema, string hint)
    {
        if (IsBinary(mediaType))
            return new TypeUse { Name = "byte[]", IsReferenceType = true };

        if (IsText(mediaType) && schema == null)
            return new TypeUse { Name = "string", IsReferenceType = true };

        if (schema == null)
            return new TypeUse { Name = "object", IsReferenceType = true };

        return Resolve(schema, hint);
    }

    void FillProperties(DataDescription model, IOpenApiSchema schema)
    {
        if (schema.Properties == null)
            return;

        var usedNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (jsonName, propertySchema) in schema.Properties)
        {
            var type = Resolve(propertySchema, model.Name + CSharpNames.ToPascal(jsonName));
            var required = schema.Required?.Contains(jsonName) == true;
            model.Properties.Add(new PropertyDescription
            {
                JsonName = jsonName,
                ClrName = Unique(CSharpNames.TypeName(jsonName), usedNames),
                TypeName = type.Name,
                IsReferenceType = type.IsReferenceType,
                Optional = !required || IsNullable(propertySchema),
                Description = propertySchema.Description
            });
        }
    }

    TypeUse Resolve(IOpenApiSchema schema, string hint)
    {
        if (schema is OpenApiSchemaReference reference)
        {
            var id = reference.Reference.Id ?? hint;
            if (_models.TryGetValue(id, out var model))
                return new TypeUse { Name = model.Name, IsReferenceType = true };

            if (_aliases.TryGetValue(id, out var alias))
                return alias;

            var target = reference.Target ?? throw new InvalidOperationException(
                $"Schema reference '{id}' could not be resolved.");
            return Resolve(target, id);
        }

        RejectComposition(schema, hint);

        var concrete = Concrete(schema.Type);
        if (concrete == JsonSchemaType.Array || schema.Items != null && concrete != JsonSchemaType.Object)
        {
            var items = schema.Items ?? throw new InvalidOperationException($"Array schema '{hint}' does not define items.");
            var item = Resolve(items, hint + "Item");
            var itemName = IsNullable(items) ? item.Name + "?" : item.Name;
            return new TypeUse { Name = "List<" + itemName + ">", IsReferenceType = true };
        }

        if (concrete == JsonSchemaType.Object || schema.Properties is { Count: > 0 })
        {
            if (_models.TryGetValue(hint, out var existing))
                return new TypeUse { Name = existing.Name, IsReferenceType = true };

            var model = new DataDescription
            {
                Name = ReserveTypeName(hint),
                Description = schema.Description
            };
            _models.Add(hint, model);
            FillProperties(model, schema);
            return new TypeUse { Name = model.Name, IsReferenceType = true };
        }

        return MapValue(schema, hint);
    }

    TypeUse MapValue(IOpenApiSchema schema, string hint)
    {
        var concrete = Concrete(schema.Type);
        var format = schema.Format;

        if (concrete == JsonSchemaType.String)
        {
            return format switch
            {
                "date" => SystemType("DateOnly"),
                "date-time" => SystemType("DateTime"),
                "uuid" => SystemType("Guid"),
                "byte" or "binary" => new TypeUse { Name = "byte[]", IsReferenceType = true },
                _ => new TypeUse { Name = "string", IsReferenceType = true }
            };
        }

        if (concrete == JsonSchemaType.Integer)
        {
            return format == "int64"
                ? new TypeUse { Name = "long", IsReferenceType = false }
                : new TypeUse { Name = "int", IsReferenceType = false };
        }

        if (concrete == JsonSchemaType.Number)
        {
            var name = format switch
            {
                "float" => "float",
                "decimal" => "decimal",
                _ => "double"
            };
            return new TypeUse { Name = name, IsReferenceType = false };
        }

        if (concrete == JsonSchemaType.Boolean)
            return new TypeUse { Name = "bool", IsReferenceType = false };

        if (concrete == JsonSchemaType.Array)
            return Resolve(schema, hint);

        return new TypeUse { Name = "object", IsReferenceType = true };
    }

    static TypeUse SystemType(string name) => new() { Name = name, IsReferenceType = false };

    string ReserveTypeName(string schemaName)
    {
        var baseName = CSharpNames.TypeName(schemaName);
        if (baseName.Length == 0)
            baseName = "Model";

        var name = baseName;
        var index = 2;
        while (!_modelNames.Add(name))
            name = baseName + index++;

        return name;
    }

    static string ReserveInterfaceName(string tag, HashSet<string> used)
    {
        var baseName = CSharpNames.InterfaceName(tag);
        var name = baseName;
        var index = 2;
        while (!used.Add(name))
            name = baseName + index++;

        return name;
    }

    static IOpenApiSchema Unwrap(IOpenApiSchema schema, string name)
    {
        if (schema is not OpenApiSchemaReference reference)
            return schema;

        return reference.Target ?? throw new InvalidOperationException(
            $"Schema reference '{reference.Reference.Id ?? name}' could not be resolved.");
    }

    static IOpenApiRequestBody Unwrap(IOpenApiRequestBody requestBody)
    {
        if (requestBody is not OpenApiRequestBodyReference reference)
            return requestBody;

        return reference.Target ?? throw new InvalidOperationException(
            $"Request body reference '{reference.Reference.Id}' could not be resolved.");
    }

    static void RejectComposition(IOpenApiSchema schema, string name)
    {
        if (schema.OneOf is { Count: > 0 })
            throw new NotSupportedException($"Schema '{name}' uses oneOf, which is not supported.");

        if (schema.AnyOf is { Count: > 0 })
            throw new NotSupportedException($"Schema '{name}' uses anyOf, which is not supported.");

        if (schema.AllOf is { Count: > 0 })
            throw new NotSupportedException($"Schema '{name}' uses allOf, which is not supported.");
    }

    static bool IsObject(IOpenApiSchema schema)
    {
        if (schema.Properties is { Count: > 0 })
            return true;

        return Concrete(schema.Type) == JsonSchemaType.Object;
    }

    static bool IsNullable(IOpenApiSchema schema)
    {
        return schema.Type?.HasFlag(JsonSchemaType.Null) == true;
    }

    static JsonSchemaType Concrete(JsonSchemaType? type)
    {
        if (type == null)
            return default;

        return type.Value & ~JsonSchemaType.Null;
    }

    static bool IsRequired(IOpenApiParameter parameter)
    {
        if (parameter.In == ParameterLocation.Path)
            return true;

        return parameter.Required;
    }

    static string AttributeName(ParameterLocation? location) => location switch
    {
        ParameterLocation.Path => "Path",
        ParameterLocation.Query => "Query",
        ParameterLocation.Header => "Header",
        ParameterLocation.Cookie => throw new NotSupportedException("Cookie parameters are not supported."),
        _ => throw new NotSupportedException($"Parameter location '{location}' is not supported.")
    };

    static string HttpAttribute(string method) => method.ToUpperInvariant() switch
    {
        "GET" => "Get",
        "POST" => "Post",
        "PUT" => "Put",
        "PATCH" => "Patch",
        "DELETE" => "Delete",
        "HEAD" => "Head",
        _ => throw new NotSupportedException($"HTTP method '{method}' is not supported.")
    };

    static string ContentAttribute(string mediaType)
    {
        if (IsJson(mediaType))
            return "JsonContent";

        if (mediaType is "application/xml" or "text/xml")
            return "XmlContent";

        if (IsText(mediaType))
            return "StringContent";

        if (IsBinary(mediaType))
            return "BinContent";

        if (mediaType == "application/x-www-form-urlencoded")
            return "FormContent";

        if (mediaType == "multipart/form-data")
            throw new NotSupportedException("multipart/form-data content is not supported.");

        throw new NotSupportedException($"Media type '{mediaType}' is not supported.");
    }

    static bool IsJson(string mediaType) =>
        mediaType == "application/json" || mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);

    static bool IsText(string mediaType) => mediaType == "text/plain";

    static bool IsBinary(string mediaType) => mediaType == "application/octet-stream";

    static List<IOpenApiParameter> MergeParameters(string path, IOpenApiPathItem pathItem, OpenApiOperation operation)
    {
        var result = new List<IOpenApiParameter>();
        var index = new Dictionary<string, int>(StringComparer.Ordinal);

        void Add(IEnumerable<IOpenApiParameter>? parameters)
        {
            if (parameters == null)
                return;

            foreach (var parameter in parameters)
            {
                var key = parameter.Name + "\n" + parameter.In;
                if (index.TryGetValue(key, out var at))
                    result[at] = parameter;
                else
                {
                    index[key] = result.Count;
                    result.Add(parameter);
                }
            }
        }

        Add(pathItem.Parameters);
        Add(operation.Parameters);

        var pathOrder = PathParameterOrder(path);
        return result
            .OrderBy(parameter => LocationOrder(parameter.In))
            .ThenBy(parameter => parameter.In == ParameterLocation.Path &&
                                 parameter.Name != null &&
                                 pathOrder.TryGetValue(parameter.Name, out var at)
                ? at
                : int.MaxValue)
            .ToList();
    }

    static Dictionary<string, int> PathParameterOrder(string path)
    {
        var order = new Dictionary<string, int>(StringComparer.Ordinal);
        var index = 0;
        for (var i = 0; i < path.Length; i++)
        {
            if (path[i] != '{')
                continue;

            var end = path.IndexOf('}', i + 1);
            if (end < 0)
                break;

            var name = path[(i + 1)..end];
            if (order.TryAdd(name, index))
                index++;

            i = end;
        }

        return order;
    }

    static int LocationOrder(ParameterLocation? location) => location switch
    {
        ParameterLocation.Path => 0,
        ParameterLocation.Query => 1,
        ParameterLocation.Header => 2,
        _ => 3
    };

    static IOpenApiResponse? SelectSuccess(OpenApiResponses? responses)
    {
        if (responses == null)
            return null;

        foreach (var code in new[] { "200", "201", "202", "203", "204" })
        {
            if (responses.TryGetValue(code, out var response))
                return response;
        }

        return responses.FirstOrDefault(pair => pair.Key is { Length: 3 } code && code[0] == '2').Value;
    }

    static (string MediaType, IOpenApiMediaType Media)? Pick(IDictionary<string, IOpenApiMediaType>? content)
    {
        if (content == null || content.Count == 0)
            return null;

        foreach (var preferred in new[]
                 {
                     "application/json",
                     "text/plain",
                     "application/xml",
                     "text/xml",
                     "application/x-www-form-urlencoded",
                     "application/octet-stream",
                     "multipart/form-data"
                 })
        {
            foreach (var (key, media) in content)
            {
                if (NormalizeMedia(key) == preferred)
                    return (preferred, media);
            }
        }

        foreach (var (key, media) in content)
        {
            var normalized = NormalizeMedia(key);
            if (normalized.EndsWith("+json", StringComparison.OrdinalIgnoreCase))
                return (normalized, media);
        }

        var first = content.First();
        return (NormalizeMedia(first.Key), first.Value);
    }

    static string NormalizeMedia(string mediaType)
    {
        var separator = mediaType.IndexOf(';');
        var value = separator < 0 ? mediaType : mediaType[..separator];
        return value.Trim().ToLowerInvariant();
    }

    static string Unique(string name, IEnumerable<string> used)
    {
        var taken = used as HashSet<string> ?? new HashSet<string>(used, StringComparer.Ordinal);
        return Unique(name, taken);
    }

    static string Unique(string name, HashSet<string> used)
    {
        var candidate = name.Length == 0 ? "value" : name;
        var index = 2;
        while (!used.Add(candidate))
            candidate = name + index++;

        return candidate;
    }
}
