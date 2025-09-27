using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object VisitPreprocessorIdentifierExpression(AmethystParser.PreprocessorIdentifierExpressionContext context)
    {
        var symbolName = context.IDENTIFIER().GetText();
        Symbol? symbol;
        try
        {
            symbol = VisitIdentifierSymbol(symbolName);
        }
        catch (SemanticException e)
        {
            throw new SyntaxException(e.Message, context);
        }
        
        if (symbol is PreprocessorVariable variable)
        {
            return variable.Value;
        }
        
        throw new SyntaxException($"Unknown symbol type '{symbol.GetType()}' in preprocessor expression. " +
                                  $"Note that stand-alone expressions can trigger this error as well.", context);
    }
}