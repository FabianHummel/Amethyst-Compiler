using Amethyst.Model;

namespace Amethyst;

public class PreprocessorIntegerResult : PreprocessorResult<int>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Int
    };

    public override bool AsBoolean => Value != 0;
    
    public override int AsInteger => Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}