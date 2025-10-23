using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override PreprocessorDatatype VisitPreprocessorType(AmethystParser.PreprocessorTypeContext context)
    {
        var basicTypeString = context.GetChild(0).GetText();

        var basicType = Enum.GetValues<BasicPreprocessorType>()
            .Cast<BasicPreprocessorType?>()
            .FirstOrDefault(t => t?.GetDescription() == basicTypeString);

        if (basicType == null)
        {
            throw new InvalidOperationException($"Unknown basic type: {basicTypeString}");
        }

        return new PreprocessorDatatype
        {
            BasicType = basicType.Value
        };
    }
}