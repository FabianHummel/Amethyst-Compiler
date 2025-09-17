namespace Amethyst.Model;

public class PreprocessorVariable : Symbol
{
    public required PreprocessorDataType DataType { get; init; }
    public required PreprocessorResult Value { get; init; }
}