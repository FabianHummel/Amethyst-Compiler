using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

/// <summary>Represents a color.</summary>
[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<Color>))]
public partial class PreprocessorColor : AbstractPreprocessorValue, IPreprocessorValue<Color>
{
    public required Color Value { get; set; }
    
    public override PreprocessorDatatype Datatype => new()
    {
        BasicType = BasicPreprocessorType.Color
    };
    
    public override bool AsBoolean => AsInteger != 0;

    public override int AsInteger => Value.Integer;
    
    public override double AsDecimal => AsInteger;
    
    public override string AsString => Value.Hex;

    public override string ToString()
    {
        return Value.Hex;
    }
}