using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

/// <summary>A preprocessor value representing a boolean.</summary>
[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<bool>))]
public partial class PreprocessorBoolean : AbstractNumericPreprocessorValue, IPreprocessorValue<bool>
{
    public required bool Value { get; set; }
    
    public override PreprocessorDatatype Datatype => new()
    {
        BasicType = BasicPreprocessorType.Bool
    };

    public override bool AsBoolean => Value;
    
    public override int AsInteger => AsBoolean ? 1 : 0;
    
    public override double AsDecimal => AsInteger;
    
    public override string AsString => Value ? "true" : "false";

    public override string ToString()
    {
        return Value ? "true" : "false";
    }
}