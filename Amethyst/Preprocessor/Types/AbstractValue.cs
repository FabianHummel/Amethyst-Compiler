using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

/// <summary>The base class for all preprocessor values. Contains a datatype and holds a reference to
/// the <see cref="Compiler" /> and a <see cref="ParserRuleContext" /> where the value was created.</summary>
public abstract partial class AbstractPreprocessorValue : IPreprocessorValue
{
    public abstract PreprocessorDatatype Datatype { get; }
    
    public required Compiler Compiler { get; init; }
    
    public required ParserRuleContext Context { get; init; }
    
    public abstract object? AbstractValue { get; }
    
    public abstract void SetValue(object? value);

    public abstract bool AsBoolean { get; }
    
    public abstract int AsInteger { get; }
    
    public abstract double AsDecimal { get; }
    
    public abstract string AsString { get; }
}