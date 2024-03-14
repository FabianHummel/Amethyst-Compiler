namespace Amethyst;

public class VariableDefinition
{
    public required string Name { get; init; }
    public required DataType Type { get; init; }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}

public class FunctionDefinition
{
    public required string Name { get; init; }
    public required string ReturnType { get; init; }
    public required IEnumerable<string> Parameters { get; init; }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}

public class ConstantDefinition
{
    public required DataType Type { get; init; }
    public required string Value { get; init; }
    public required string Name { get; init; }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public class GenerationContext
{
    public List<string> TickFunctions { get; init; } = new();
    public List<string> LoadFunctions { get; init; } = new();
    public HashSet<VariableDefinition> Variables { get; init; } = new();
    public HashSet<FunctionDefinition> Functions { get; init; } = new();
    public HashSet<ConstantDefinition> Constants { get; init; } = new();
    public required string OutDir { get; init; }
    public required string RootNamespace { get; init; }
    public string CurrentNamespace { get; set; } = "";
    public string? CurrentFunction { get; set; }
    
    public void AddConstant(Constant constant, out ConstantDefinition result)
    {
        result = new ConstantDefinition
        {
            Type = constant.Type,
            Value = constant.Value,
            Name = "c" + Constants.Count
        };
        Constants.Add(result);
    }
    
    public ConstantDefinition? GetConstant(Constant constant)
    {
        Constants.TryGetValue(new ConstantDefinition
        {
            Value = constant.Value,
            Type = constant.Type,
            Name = ""
        }, out var result);
        return result;
    }
}