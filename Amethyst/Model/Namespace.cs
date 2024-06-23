using System.Text;
using Amethyst.Language;

namespace Amethyst.Model;

public class Namespace
{
    public required Context Context { get; init; }
    public required string Name { get; init; }
    public required Namespace? Parent { get; init; }
    public Dictionary<string, AmethystParser.Function_declarationContext> Functions { get; } = new();
    public Dictionary<string, AmethystParser.Variable_declarationContext> Variables { get; } = new();
    public Dictionary<string, AmethystParser.Record_declarationContext> Records { get; } = new();
    public Dictionary<string, Namespace> Namespaces { get; } = new();
    
    public string SourceFilePath => Path.Combine(Context.SourcePath, Name).Replace('\\', '/');

    public string McFunctionPath
    {
        get
        {
            var sb = new StringBuilder("/");
            var current = this;
            while (current.Parent is not null)
            {
                sb.Insert(0, $"/{current.Name}");
                current = current.Parent;
            }
            return $"{current.Name}:{sb.ToString()[1..]}";
        }
    }
}