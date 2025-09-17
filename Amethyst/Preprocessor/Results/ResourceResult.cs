using Amethyst.Model;

namespace Amethyst;

public class PreprocessorResourceResult : PreprocessorResult<string>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Resource
    };
}