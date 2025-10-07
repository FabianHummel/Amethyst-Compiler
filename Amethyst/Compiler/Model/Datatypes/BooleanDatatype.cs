namespace Amethyst.Model;

public class BooleanDatatype : AbstractScoreboardDatatype
{
    public override BasicType BasicType => BasicType.Bool;

    public override string StorageModifier => "byte 1";
}