using Antlr4.Runtime;

namespace Amethyst.Model;

public abstract class PreprocessorResult
{
    public abstract PreprocessorDataType DataType { get; }
    public required Compiler Compiler { get; init; }
    public required ParserRuleContext Context { get; init; }
}

public abstract class PreprocessorResult<T> : PreprocessorResult
{
    public required T Value { get; init; }
}