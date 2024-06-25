using Amethyst.Language;
using Amethyst.Utility;

namespace Amethyst.Model;

public class Namespace
{
    public required Context Context { get; init; }
    public required Scope Scope { get; init; }
    public List<AmethystParser.FileContext> Files { get; } = new();
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
}