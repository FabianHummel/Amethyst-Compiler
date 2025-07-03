
namespace Amethyst.Model;

public class Record
{
    public required List<string> Attributes { get; init; }
    public required string Name { get; init; }
    public required DataType DataType { get; init; }
    public required RuntimeValue? InitialValue { get; init; }
}