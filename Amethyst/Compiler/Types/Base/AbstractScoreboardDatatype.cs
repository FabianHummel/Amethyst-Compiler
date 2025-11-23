using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractScoreboardDatatype : AbstractDatatype
{
    public override DataLocation DataLocation => Modifier == null 
        ? DataLocation.Scoreboard 
        : DataLocation.Storage;

    public abstract string StorageModifier { get; }
}