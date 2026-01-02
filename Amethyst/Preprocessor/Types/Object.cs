using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

[ForwardDefaultInterfaceMethods(typeof(IPreprocessorValue<Dictionary<string, IPreprocessorValue>>))]
public partial class PreprocessorObject : AbstractPreprocessorValue, IPreprocessorValue<Dictionary<string, IPreprocessorValue>>
{
    public required Dictionary<string, IPreprocessorValue> Value { get; set; }

    public override PreprocessorDatatype Datatype => new()
    {
        BasicType = BasicPreprocessorType.Object
    };

    public override bool AsBoolean => Value.Count > 0;
    
    public override int AsInteger => Value.Count;
    
    public override double AsDecimal => AsInteger;
    
    public override string AsString => $"{{{string.Join(", ", Value.Select(kv => $"{kv.Key}: {kv.Value}"))}}}";
}