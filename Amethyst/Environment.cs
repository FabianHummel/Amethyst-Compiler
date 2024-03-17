namespace Amethyst;

public class Environment
{
    private string Scope { get; init; }
    public string CurrentFunction { get; init; }
    private Environment? Enclosing { get; }
    public List<string> TickingFunctions { get; init; } = new();
    public List<string> InitializingFunctions { get; init; } = new();
    
    public Environment(string currentFunction)
    {
        Enclosing = null;
        Scope = "";
        CurrentFunction = currentFunction;
    }

    public Environment(Environment enclosing, string currentFunction, string scope = "")
    {
        Enclosing = enclosing;
        Scope = scope;
        CurrentFunction = currentFunction;
    }

    public string Namespace => Path.Combine(Enclosing?.Namespace ?? "", Scope);
}