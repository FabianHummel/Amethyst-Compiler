using Amethyst.Model;

namespace Amethyst;

public partial class PreprocessorDecimal : AbstractNumericPreprocessorValue, IPreprocessorValue<double>
{
    public required double Value { get; set; }
    
    public override PreprocessorDataType DataType => new()
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