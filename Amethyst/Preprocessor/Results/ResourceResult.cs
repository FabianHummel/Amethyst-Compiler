using Amethyst.Model;

namespace Amethyst;

public class PreprocessorResourceResult : PreprocessorResult<string>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Resource
    };

    public override bool AsBoolean => throw new SyntaxException("Cannot interpret a resource path as a boolean value.", Context);
    
    public override int AsInteger => throw new SyntaxException("Cannot interpret a resource path as an integer value.", Context);

    public override string ToString()
    {
        return $"`{Value}`";
    }
}