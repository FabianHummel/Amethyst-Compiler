namespace Amethyst.Model;

public class PreprocessorVariable : Symbol
{
    public required PreprocessorDataType DataType { get; init; }
    public required AbstractPreprocessorValue Value { get; init; }
}