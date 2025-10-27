using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Enters a new yielding scope for the given context and visits it, returning all yielded
    /// results of type T.<br />
    /// <inheritdoc cref="AmethystParserBaseVisitor{Result}.VisitPreprocessorYieldingStatement" />
    /// </summary>
    /// <typeparam name="T">The allowed type of yielded results.</typeparam>
    public IEnumerable<T> VisitPreprocessorYieldingStatement<T>(AmethystParser.PreprocessorYieldingStatementContext context)
        where T : ParserRuleContext
    {
        using var scope = new YieldingScope(this, typeof(T));
        Visit(context);
        return scope.Result.Cast<T>();
    }
}