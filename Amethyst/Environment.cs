namespace Amethyst;

public class Environment
{
    private string Scope { get; init; }
    private Environment? Enclosing { get; }
    public IDictionary<string, object?> Values { get; } = new Dictionary<string, object?>();
    public List<string> TickingFunctions { get; init; } = new();
    public List<string> InitializingFunctions { get; init; } = new();
    
    public Environment()
    {
        Enclosing = null;
        Scope = "";
    }

    public Environment(Environment enclosing, string scope = "")
    {
        Enclosing = enclosing;
        Scope = scope;
    }

    public string Namespace => Path.Combine(Enclosing?.Namespace ?? "", Scope);
}