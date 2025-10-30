namespace Amethyst.Model;

/// <summary>A variable defined in the preprocessor.</summary>
/// <seealso cref="Variable" />
public class PreprocessorVariable : Symbol
{
    public required PreprocessorDatatype Datatype { get; init; }
    public required AbstractPreprocessorValue Value { get; init; }
}