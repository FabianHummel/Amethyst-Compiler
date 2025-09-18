using Amethyst.Model;

namespace Amethyst;

public class PreprocessorIntegerResult : PreprocessorResult<int>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Int
    };

    public override bool AsBoolean => Value != 0;
}