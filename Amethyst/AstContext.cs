namespace Amethyst;

public class VariableDefinition
{
    public required string Name { get; init; }
    public required DataType Type { get; init; }
}

public class FunctionDefinition
{
    public required string Name { get; init; }
    public required string ReturnType { get; init; }
    public required IEnumerable<string> Parameters { get; init; }
}

public class AstContext
{
    public required List<VariableDefinition> Variables { get; init; }
    public required List<FunctionDefinition> Functions { get; init; }
    public required string OutDir { get; init; }
    public required string Namespace { get; set; }
    public required string? CurrentFunction { get; set; }
}