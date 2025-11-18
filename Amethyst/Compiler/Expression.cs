using Amethyst.Language;
using Antlr4.Runtime.Tree;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>Utility method that parses any expression and returns an <see cref="AbstractValue" /> that
    ///     holds detailed information about the created value.</p>
    ///     <p>Visit a parse tree produced by <see cref="AmethystParser.expression" />.
    ///     <para>The default implementation returns the result of calling
    ///     <see cref="AbstractParseTreeVisitor{Result}.VisitChildren(IRuleNode)" /> on
    ///     <paramref name="context" />.</para>
    ///     </p>
    /// </summary>
    /// <param name="context">The parse tree.</param>
    /// <returns>The visitor result.</returns>
    public AbstractValue VisitExpression(AmethystParser.ExpressionContext context)
    {
        return (AbstractValue)Visit(context)!;
    }
}