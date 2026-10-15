namespace MyLab.ApiClient.Generator;

sealed class ApiDescription
{
    public required string Namespace { get; init; }
    public string? Binding { get; init; }
    public string? Version { get; init; }
    public List<ServerDescription> Servers { get; } = [];
    public List<DataDescription> Schemas { get; } = [];
    public List<ContractDescription> Contracts { get; } = [];
}

sealed class ServerDescription
{
    public required string Url { get; init; }
    public string? Description { get; init; }
}

sealed class DataDescription
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public List<PropertyDescription> Properties { get; } = [];
}

sealed class PropertyDescription
{
    public required string JsonName { get; init; }
    public required string ClrName { get; init; }
    public required string TypeName { get; init; }
    public required bool IsReferenceType { get; init; }
    public required bool Optional { get; init; }
    public string? Description { get; init; }
}

sealed class ContractDescription
{
    public required string Name { get; init; }
    public required string Summary { get; init; }
    public List<OperationDescription> Operations { get; } = [];
}

sealed class OperationDescription
{
    public required string Name { get; init; }
    public required string HttpAttribute { get; init; }
    public required string Url { get; init; }
    public required string ReturnType { get; set; }
    public string? Summary { get; init; }
    public string? Remarks { get; set; }
    public string? Returns { get; set; }
    public bool Deprecated { get; init; }
    public List<ParameterDescription> Parameters { get; } = [];
}

sealed class ParameterDescription
{
    public required string Name { get; init; }
    public required string Attribute { get; init; }
    public string? AttributeArgument { get; init; }
    public required string TypeName { get; init; }
    public required bool Optional { get; init; }
    public string? Description { get; init; }
}

sealed class TypeUse
{
    public required string Name { get; init; }
    public required bool IsReferenceType { get; init; }
}
