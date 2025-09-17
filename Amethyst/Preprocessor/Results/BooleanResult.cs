using Amethyst.Model;

namespace Amethyst;

public class PreprocessorBooleanResult : PreprocessorResult<bool>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Bool
    };
}