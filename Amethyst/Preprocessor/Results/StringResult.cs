using Amethyst.Model;

namespace Amethyst;

public class PreprocessorStringResult : PreprocessorResult<string>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.String
    };
}