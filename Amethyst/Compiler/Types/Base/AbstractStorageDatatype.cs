using Amethyst.Model;

namespace Amethyst;

public abstract class AbstractStorageDatatype : AbstractDatatype
{
    public override DataLocation DataLocation => DataLocation.Storage;
}