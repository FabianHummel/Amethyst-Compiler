namespace Amethyst.Model;

public class Variable
{
    public required List<string> Attributes { get; init; }
    public required int Location { get; init; }
    public required DataType DataType { get; init; }
}