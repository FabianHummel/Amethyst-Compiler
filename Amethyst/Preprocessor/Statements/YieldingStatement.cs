using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public IEnumerable<T> VisitPreprocessorYieldingStatement<T>(AmethystParser.PreprocessorYieldingStatementContext context)
        where T : ParserRuleContext
    {
        using var scope = new YieldingScope(this, typeof(T));
        Visit(context);
        return scope.Result.Cast<T>();
    }
}