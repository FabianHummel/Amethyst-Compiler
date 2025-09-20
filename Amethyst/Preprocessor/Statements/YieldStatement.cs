using Amethyst.Language;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorYieldStatement(AmethystParser.PreprocessorYieldStatementContext context)
    {
        if (YieldingScope is not { } scope)
        {
            throw new SyntaxException("Can only yield within enclosing context.", context);
        }

        ParserRuleContext? rule = null;
        
        rule ??= context.selectorElement();
        rule ??= context.recordSelectorElement();
        rule ??= context.arrayElement();
        rule ??= context.objectElement();
        
        scope.Result.Add(rule);
        
        return null;
    }
}