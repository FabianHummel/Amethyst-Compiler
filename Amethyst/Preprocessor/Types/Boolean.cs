using Amethyst.Model;

namespace Amethyst;

public partial class PreprocessorBoolean : AbstractNumericPreprocessorValue, IPreprocessorValue<bool>
{
    public required bool Value { get; set; }
    
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Bool
    };

    public override bool AsBoolean => Value;
    
    public override int AsInteger => Value ? 1 : 0;

    public override string ToString()
    {
        return Value ? "true" : "false";
    }
}