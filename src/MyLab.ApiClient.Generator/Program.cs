using MyLab.ApiClient.Generator;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: MyLab.ApiClient.Generator <openapi.yaml> --output <directory> [--namespace <name>]");
    return 1;
}

string? input = null;
string? output = null;
string? ns = null;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--output" or "-o":
            output = Next(args, ref i);
            break;
        case "--namespace" or "-n":
            ns = Next(args, ref i);
            break;
        default:
            if (input != null)
            {
                Console.Error.WriteLine("Unexpected argument '" + args[i] + "'.");
                return 1;
            }

            input = args[i];
            break;
    }
}

if (input == null || output == null)
{
    Console.Error.WriteLine("Usage: MyLab.ApiClient.Generator <openapi.yaml> --output <directory> [--namespace <name>]");
    return 1;
}

var yaml = File.ReadAllText(input);
var document = ApiContractGenerator.Generate(yaml, ns == null ? null : new GenerationOptions { Namespace = ns });
Directory.CreateDirectory(output);

foreach (var file in document.Files)
{
    var path = Path.Combine(output, file.Name);
    File.WriteAllText(path, file.Code);
    Console.WriteLine(path);
}

return 0;

static string Next(string[] args, ref int index)
{
    if (index + 1 >= args.Length)
        throw new ArgumentException("Missing value for " + args[index] + ".");

    index++;
    return args[index];
}
