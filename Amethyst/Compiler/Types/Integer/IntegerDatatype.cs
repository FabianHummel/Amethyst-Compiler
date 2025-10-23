using Amethyst.Model;

namespace Amethyst;

public class IntegerDatatype : AbstractScoreboardDatatype
{
    public override BasicType BasicType => BasicType.Int;

    public override string StorageModifier => "int 1";
}