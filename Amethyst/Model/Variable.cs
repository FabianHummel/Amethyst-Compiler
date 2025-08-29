namespace Amethyst.Model;

public class Variable : Symbol
{
    public required int Location { get; init; }
    public required DataType DataType { get; init; }
}