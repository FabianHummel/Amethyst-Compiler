using Amethyst.Utility;

namespace Amethyst.Model;

public class Namespace
{
    public required Context Context { get; init; }
    public required Scope Scope { get; init; }
    public List<SourceFile> Files { get; } = new();
    public Dictionary<string, Function> Functions { get; } = new();

    public string GenerateFunctionName()
    {
        var randomString = StringGenerator.GenerateRandomString(8);
        if (Functions.ContainsKey(randomString))
        {
            return GenerateFunctionName();
        }
        return randomString;
    }

    public void AddInitCode(string code)
    {
        code = code.TrimEnd() + "\n";
        var initFunction = Functions["_load"];
        File.AppendAllText(initFunction.FilePath, code);
    }
}