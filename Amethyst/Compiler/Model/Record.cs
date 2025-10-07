
namespace Amethyst.Model;

public class Record : Symbol
{
    public required string Name { get; init; }
    public required AbstractDatatype Datatype { get; init; }
    public required IRuntimeValue? InitialValue { get; init; }
    public required HashSet<string> Attributes { get; init; }
}