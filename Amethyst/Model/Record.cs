
namespace Amethyst.Model;

public class Record : Symbol
{
    public required string Name { get; init; }
    public required DataType DataType { get; init; }
    public required RuntimeValue? InitialValue { get; init; }
}