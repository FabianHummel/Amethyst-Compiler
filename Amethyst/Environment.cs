namespace Amethyst;

public class Environment
{
    public Environment? Enclosing { get; }
    public IDictionary<string, object?> Values { get; } = new Dictionary<string, object?>();
    
    public Environment()
    {
        Enclosing = null;
    }
    
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }
}