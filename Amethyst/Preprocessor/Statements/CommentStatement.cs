using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Parses a raw MCF comment. The comment is embedded as-is into the resulting datapack code.
    ///     This may be useful for numerous reasons.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><c>COMMENT "Hello World!"</c> → <c># Hello World!</c></example>
    public override object? VisitPreprocessorCommentStatement(AmethystParser.PreprocessorCommentStatementContext context)
    {
        var result = VisitPreprocessorExpression(context.preprocessorExpression());
        this.AddCode($"# {result.AbstractValue}");
        return null;
    }
}