using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

/// <summary>A preprocessor value representing a decimal number.</summary>
[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<double>))]
public partial class PreprocessorDecimal : AbstractNumericPreprocessorValue, IPreprocessorValue<double>
{
    public required double Value { get; set; }
    
    public override PreprocessorDatatype Datatype => new()
    {
        BasicType = BasicPreprocessorType.Dec
    };

    public override bool AsBoolean => Value != 0.0d;
    
    public override int AsInteger => (int)Value;
    
    public override double AsDecimal => Value;

    public override string ToString()
    {
        return Value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }

}