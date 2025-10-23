namespace Amethyst.Model;

public class PreprocessorVariable : Symbol
{
    public required PreprocessorDatatype Datatype { get; init; }
    public required AbstractPreprocessorValue Value { get; init; }
}