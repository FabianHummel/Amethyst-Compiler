using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public abstract partial class AbstractResult
{
    public abstract DataType DataType { get; }
    
    public required string Location { get; init; }
    public required Compiler Compiler { get; init; }
    public required ParserRuleContext Context { get; init; }
    public bool IsTemporary { get; init; }
    
    public virtual BoolResult ToBool => throw new SyntaxException($"Conversion of {DataType} to {BasicType.Bool} not permitted.", Context);
    public virtual IntResult ToNumber => throw new SyntaxException($"Conversion of {DataType} to {BasicType.Int} not permitted.", Context);
    
    protected int MemoryLocation
    {
        get => Compiler.MemoryLocation;
        set => Compiler.MemoryLocation = value;
    }

    protected void AddCode(string code) => Compiler.AddCode(code);
    protected void AddInitCode(string code) => Compiler.AddInitCode(code);
}
