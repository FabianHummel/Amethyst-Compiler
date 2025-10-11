using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractScoreboardDatatype : AbstractDatatype
{
    public override DataLocation DataLocation => DataLocation.Scoreboard;

    public abstract string StorageModifier { get; }
}