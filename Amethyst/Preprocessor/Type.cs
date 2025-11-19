using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>The textual representation of preprocessor types is converted to its enum equivalent by
    ///     comparing the enum's description attribute.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><c>INT</c> → <see cref="BasicPreprocessorType.Int" /><br /> <c>DEC</c> →
    /// <see cref="BasicPreprocessorType.Dec" /></example>
    /// <exception cref="InvalidOperationException">The type is unknown.</exception>
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