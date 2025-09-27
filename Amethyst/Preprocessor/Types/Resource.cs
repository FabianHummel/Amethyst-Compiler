using Amethyst.Model;

namespace Amethyst;

public partial class PreprocessorResource : AbstractPreprocessorValue, IPreprocessorValue<string>
{
    public required string Value { get; set; }
    
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