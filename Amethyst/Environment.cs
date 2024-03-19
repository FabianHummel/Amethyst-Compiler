namespace Amethyst;

public class Environment
{
    public class Variable
    {
        public string Name { get; }
        public Subject Subject { get; set; }
        
        public Variable(string name, Subject subject)
        {
            Name = name;
            Subject = subject;
        }
    }
    
    private string Scope { get; }
    public string CurrentFunction { get; }
    private Environment? Enclosing { get; }
    public IList<string> TickingFunctions { get; } = new List<string>();
    public IList<string> InitializingFunctions { get; } = new List<string>();
    
    private IDictionary<string, Variable> Values { get; } = new Dictionary<string, Variable>();
    private ISet<string> VariableNames { get; } = new HashSet<string>();
    
    public string Namespace => Path.Combine(Enclosing?.Namespace ?? "", Scope);
    
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

    public Variable AddVariable(string name, Subject subject)
    {
        var variable = new Variable($"v{name}", subject);
        Values[name] = variable;
        return variable;
    }

    public Variable GetVariable(string name)
    {
        if (Values[name] is { } value)
        {
            return value;
        }
        
        if (Enclosing?.GetVariable(name) is { } variable)
        {
            return variable;
        }

        throw new Exception($"Undefined variable '{name}'");
    }
}