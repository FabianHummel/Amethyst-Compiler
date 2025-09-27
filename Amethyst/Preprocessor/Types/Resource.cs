using Amethyst.Model;

namespace Amethyst;

public partial class PreprocessorResource : AbstractPreprocessorValue, IPreprocessorValue<string>
{
    public required string Value { get; set; }
    
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Resource
    };
    
    private SyntaxException _cannotInterpretException => new("Cannot interpret resource paths as an arithmetic value.", Context);
    
    public override bool AsBoolean => throw _cannotInterpretException;
    
    public override int AsInteger => throw _cannotInterpretException;
    
    public override double AsDecimal => throw _cannotInterpretException;

    public override string ToString()
    {
        return $"`{Value}`";
    }
}