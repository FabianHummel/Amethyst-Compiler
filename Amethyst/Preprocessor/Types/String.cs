using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

/// <summary>Represents a string value in the preprocessor.</summary>
[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<string>))]
public partial class PreprocessorString : AbstractPreprocessorValue, IPreprocessorValue<string>
{
    public required string Value { get; set; }
    
    public override PreprocessorDatatype Datatype => new()
    {
        BasicType = BasicPreprocessorType.String
    };

    public override bool AsBoolean => !string.IsNullOrEmpty(Value);

    public override int AsInteger => Value.Length;
    
    public override double AsDecimal => AsInteger;

    public override string ToString()
    {
        return Value;
    }
}