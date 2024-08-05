using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractResult
{
    public abstract DataType DataType { get; }
    
    public required string Location { get; init; }
    public required Compiler Compiler { get; init; }
    public required ParserRuleContext Context { get; init; }

    public virtual AbstractResult? ToBool => null;
    public virtual AbstractResult? ToNumber => null;
}
