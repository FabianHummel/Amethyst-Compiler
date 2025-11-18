using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

/// <summary>Represents an Amethyst resource path.</summary>
/// <example><c>`minecraft:path/to/resource`</c><br /> <c>`my_namespace:path/to/function`</c></example>
[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<string>))]
public partial class PreprocessorResource : AbstractPreprocessorValue, IPreprocessorValue<string>
{
    public required string Value { get; set; }
    
    public override PreprocessorDatatype Datatype => new()
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