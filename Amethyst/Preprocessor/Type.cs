using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override PreprocessorDatatype VisitPreprocessorType(AmethystParser.PreprocessorTypeContext context)
    {
        var basicTypeString = context.GetChild(0).GetText();
        var basicType = basicTypeString switch
        {
            "INT" => BasicPreprocessorType.Int,
            "STRING" => BasicPreprocessorType.String,
            "BOOL" => BasicPreprocessorType.Bool,
            "DEC" => BasicPreprocessorType.Dec,
            "RESOURCE" => BasicPreprocessorType.Resource,
            _ => throw new SyntaxException($"Invalid basic type '{basicTypeString}'.", context)
        };

        return new PreprocessorDatatype
        {
            BasicType = basicType
        };
    }
}