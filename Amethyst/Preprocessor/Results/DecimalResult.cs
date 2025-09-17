using Amethyst.Model;

namespace Amethyst;

public class PreprocessorDecimalResult : PreprocessorResult<double>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Dec
    };
}