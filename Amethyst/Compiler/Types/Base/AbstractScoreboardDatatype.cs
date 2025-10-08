namespace Amethyst.Model;

public abstract class AbstractScoreboardDatatype : AbstractDatatype
{
    public override DataLocation DataLocation => DataLocation.Scoreboard;

    public abstract string StorageModifier { get; }
}