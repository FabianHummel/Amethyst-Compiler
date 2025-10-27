using Amethyst.Language;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Yields a value from the enclosing yielding scope.<br /><inheritdoc /></summary>
    /// <exception cref="SyntaxException">Thrown if there is no enclosing yielding scope.</exception>
    /// <seealso cref="Model.YieldingScope" />
    public override object? VisitPreprocessorYieldStatement(AmethystParser.PreprocessorYieldStatementContext context)
    {
        if (YieldingScope is not { } scope)
        {
            throw new SyntaxException("Can only yield within enclosing context.", context);
        }

        var rule = context.GetRuleContext<ParserRuleContext>(0);
        scope.Yield(rule);
        
        return null;
    }
}