using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractPreprocessorValue : IPreprocessorValue
{
    public abstract PreprocessorDataType DataType { get; }
    
    public required Compiler Compiler { get; init; }
    
    public required ParserRuleContext Context { get; init; }
    
    public abstract object? AbstractValue { get; }
    
    public abstract void SetValue(object? value);

    public abstract bool AsBoolean { get; }
    
    public abstract int AsInteger { get; }
    
    public abstract double AsDecimal { get; }
}