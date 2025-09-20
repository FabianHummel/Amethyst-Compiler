namespace Amethyst.Model;

public class Namespace
{
    public required Context Context { get; init; }
    public required string Name { get; init; }
    public Dictionary<string, SourceFolder> Registries { get; } = new();

    public void AddInitCode(string code)
    {
        
    }
}