using Amethyst.Model;

namespace Amethyst;

public class PreprocessorDecimalResult : PreprocessorResult<double>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Dec
    };

    public override bool AsBoolean => Value != 0.0d;
}