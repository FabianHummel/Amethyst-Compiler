namespace Amethyst.Model;

public class Function
{
    public required string Name { get; init; }
    public required List<string> Attributes { get; init; }
    public required Scope Scope { get; init; }

    public string McFunctionPath => Scope.McFunctionPath + Name;
}