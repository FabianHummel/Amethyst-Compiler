using System.Diagnostics.CodeAnalysis;

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
    public Environment? Enclosing { get; }
    public ISet<string> TickingFunctions { get; } = new HashSet<string>();
    public ISet<string> InitializingFunctions { get; } = new HashSet<string>();
    public int IfCounter { get; set; }
    public int LoopCounter { get; set; }
    public int BinaryCounter { get; set; }
    
    private IDictionary<string, Variable> Values { get; } = new Dictionary<string, Variable>();

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

    private bool IsDefined(string targetName)
    {
        return Values.ContainsKey(targetName) || Enclosing?.IsDefined(targetName) == true;
    }
    
    public string GetUniqueName()
    {
        var index = 0;
        string name;
        do
        {
            name = "v" + Convert.ToString(index++);
        } while (IsDefined(name));
        return name;
    }

    public bool AddVariable(string targetName, Subject subject, out Variable variable)
    {
        var name = GetUniqueName(); 
        variable = new Variable(name, subject);
        return Values.TryAdd(targetName, variable);
    }
    
    public bool TryGetVariable(string targetName, [NotNullWhen(true)] out Variable? variable)
    {
        if (Values.TryGetValue(targetName, out variable))
        {
            return true;
        }
        
        if (Enclosing?.TryGetVariable(targetName, out variable) == true)
        {
            return true;
        }

        return false;
    }
    
    public bool RemoveVariable(string targetName)
    {
        if (Values.Remove(targetName))
        {
            return true;
        }
        
        Enclosing?.RemoveVariable(targetName);
        return false;
    }
}