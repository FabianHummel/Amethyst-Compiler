using Amethyst.Model;

namespace Amethyst;

public class PreprocessorResourceResult : PreprocessorResult<string>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Resource
    };

    public override bool AsBoolean => throw new SyntaxException("Cannot interpret a resource path to a boolean value.", Context);
}