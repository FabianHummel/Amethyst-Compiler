using Amethyst.Model;

namespace Amethyst;

public class PreprocessorIntegerResult : PreprocessorResult<int>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Int
    };
}