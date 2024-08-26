using Amethyst.Model;

namespace Amethyst;

public class ObjectResult : AbstractResult
{
    public required BasicType BasicType { get; init; }
    
    public override DataType DataType => new()
    {
        BasicType = BasicType,
        Modifier = Modifier.Object
    };

    protected override string GetSubstitutionModifier(object index)
    {
        return $".{index}";
    }
}