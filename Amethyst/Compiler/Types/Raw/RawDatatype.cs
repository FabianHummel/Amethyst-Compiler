using Amethyst.Model;

namespace Amethyst;

public class RawDatatype : AbstractDatatype
{
    public RawDatatype(DataLocation dataLocation)
    {
        DataLocation = dataLocation;
    }

    public override BasicType BasicType => BasicType.Raw;
    
    public override DataLocation DataLocation { get; }
    
    public required string Namespace { get; init; }
    
    public required string Name { get; init; }
    
    public static implicit operator Location (RawDatatype datatype) => new()
    {
        Namespace =  datatype.Namespace,
        Name = datatype.Name,
        DataLocation =  datatype.DataLocation
    };
}