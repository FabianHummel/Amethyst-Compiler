namespace Amethyst.Model;

public class Namespace
{
    public required Context Context { get; init; }
    public required Scope Scope { get; init; }
    public List<SourceFile> Files { get; } = new();
    public Dictionary<string, Function> Functions { get; } = new();

    public string GetFunctionName(string debugName)
    {
        if (Context.Flags.HasFlag(Flags.Debug))
        {
            return debugName;
        }
        
        return Functions.Count == 0 ? "_load" : "_func";
    }

    public void AddInitCode(string code)
    {
        code = code.TrimEnd() + "\n";
        var initFunction = Functions["_load"];
        File.AppendAllText(initFunction.FilePath, code);
    }
}