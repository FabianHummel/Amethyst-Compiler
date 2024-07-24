namespace Amethyst.Model;

public class Variable
{
    public required List<string> Attributes { get; init; }
    public required string Location { get; init; }
    public required Type Type { get; init; }
}