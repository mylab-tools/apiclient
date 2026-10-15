using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MyLab.ApiClient.Generator;

namespace GeneratorTests;

static class GeneratedCompiler
{
    public static CompiledAssembly Compile(GeneratedDocument document)
    {
        var trees = document.Files
            .Select(file => CSharpSyntaxTree.ParseText(file.Code, path: file.Name))
            .ToList();

        var compilation = CSharpCompilation.Create(
            "GeneratedApi_" + Guid.NewGuid().ToString("N"),
            trees,
            MetadataReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable));

        using var pe = new MemoryStream();
        var emit = compilation.Emit(pe);
        if (!emit.Success)
        {
            var errors = string.Join(
                Environment.NewLine,
                emit.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error));
            var sources = string.Join(
                Environment.NewLine + Environment.NewLine,
                document.Files.Select(file => "// " + file.Name + Environment.NewLine + file.Code));
            throw new InvalidOperationException(errors + Environment.NewLine + Environment.NewLine + sources);
        }

        pe.Position = 0;
        var assembly = new GeneratedLoadContext().LoadFromStream(pe);
        return new CompiledAssembly(assembly);
    }

    static IEnumerable<MetadataReference> MetadataReferences()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trusted)
        {
            foreach (var path in trusted.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                paths.Add(path);
        }

        Add(paths, typeof(object).Assembly);
        Add(paths, typeof(MyLab.ApiClient.Contracts.Attributes.ForContract.ApiContractAttribute).Assembly);
        Add(paths, typeof(Newtonsoft.Json.JsonPropertyAttribute).Assembly);
        Add(paths, typeof(System.Text.Json.Serialization.JsonPropertyNameAttribute).Assembly);

        return paths.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path));
    }

    static void Add(HashSet<string> paths, Assembly assembly)
    {
        if (!string.IsNullOrEmpty(assembly.Location))
            paths.Add(assembly.Location);
    }

    sealed class GeneratedLoadContext : AssemblyLoadContext
    {
        public GeneratedLoadContext() : base(isCollectible: false)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return Default.Assemblies.FirstOrDefault(assembly => assembly.GetName().Name == assemblyName.Name);
        }
    }
}

sealed class CompiledAssembly
{
    public CompiledAssembly(Assembly assembly)
    {
        Assembly = assembly;
    }

    public Assembly Assembly { get; }
}
