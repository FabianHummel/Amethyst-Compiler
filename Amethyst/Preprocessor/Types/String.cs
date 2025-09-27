using Amethyst.Model;

namespace Amethyst;

public partial class PreprocessorString : AbstractPreprocessorValue, IPreprocessorValue<string>
{
    public required string Value { get; set; }
    
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.String
    };

    public override bool AsBoolean => !string.IsNullOrEmpty(Value);

    public override int AsInteger => Value.Length;

    public override string ToString()
    {
        return Value;
    }
}