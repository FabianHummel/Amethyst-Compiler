namespace Amethyst.Model;

public class UnknownDatatype : AbstractDatatype
{
    public UnknownDatatype(DataLocation dataLocation)
    {
        DataLocation = dataLocation;
    }

    public override BasicType BasicType => BasicType.Unknown;
    
    public override DataLocation DataLocation { get; }
}