namespace Amethyst.Model;

/// <summary>A variable defined in the preprocessor.</summary>
/// <seealso cref="Variable" />
public class PreprocessorVariable : Symbol
{
    public PreprocessorDatatype? Datatype { get; init; }
    public AbstractPreprocessorValue? Value { get; set; }
}