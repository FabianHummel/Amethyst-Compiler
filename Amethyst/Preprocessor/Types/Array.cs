using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<IPreprocessorValue[]>))]
public partial class PreprocessorArray : AbstractPreprocessorValue, IPreprocessorValue<IPreprocessorValue[]>
{
    public required IPreprocessorValue[] Value { get; set; }

    public override PreprocessorDatatype Datatype => new()
    {
        BasicType = BasicPreprocessorType.Array
    };
    
    public override bool AsBoolean => Value.Length > 0;
    
    public override int AsInteger => Value.Length;
    
    public override double AsDecimal => AsInteger;

    public override string AsString => $"[{string.Join(", ", Value)}]";
}