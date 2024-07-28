namespace Amethyst.Model.Types;

public abstract partial class AbstractResult
{
    public abstract DataType DataType { get; }
    public required string Location { get; init; }
    public required Compiler Compiler { get; init; }

    public virtual AbstractResult? ToBool => null;
    public virtual AbstractResult? ToNumber => null;
}
