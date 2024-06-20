namespace Amethyst.Model;

public class Namespace
{
    public required Context Context { get; init; }
    public required string Name { get; init; }
    public List<Stmt.Function> Functions { get; } = new();
    public List<Stmt.Var> Variables { get; } = new();
    public List<Namespace> Namespaces { get; } = new();
    
    public string SourcePath => Path.Combine(Context.SourcePath, Name).Replace('\\', '/');
}