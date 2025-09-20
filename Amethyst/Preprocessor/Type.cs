using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override PreprocessorDataType VisitPreprocessorType(AmethystParser.PreprocessorTypeContext context)
    {
        var basicType = context.GetChild(0).GetText() switch
        {
            "INT" => BasicPreprocessorType.Int,
            "STRING" => BasicPreprocessorType.String,
            "BOOL" => BasicPreprocessorType.Bool,
            "DEC" => BasicPreprocessorType.Dec,
            "RESOURCE" => BasicPreprocessorType.Resource,
            _ => throw new SyntaxException("Expected basic preprocessor type.", context)
        };

        return new PreprocessorDataType
        {
            BasicType = basicType
        };
    }
}