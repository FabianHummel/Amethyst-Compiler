using Amethyst.Model;

namespace Amethyst;

public partial class PreprocessorInteger : AbstractNumericPreprocessorValue, IPreprocessorValue<int>
{
    public required int Value { get; set; }
    
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Int
    };

    public override bool AsBoolean => Value != 0;
    
    public override int AsInteger => Value;
    
    public override double AsDecimal => Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}