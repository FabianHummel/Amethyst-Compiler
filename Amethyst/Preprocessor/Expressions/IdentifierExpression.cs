using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary><p>Resolves a preprocessor identifier expression to its corresponding symbol value.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">The symbol is of an unknown type.</exception>
    /// <seealso cref="VisitIdentifierExpression" />
    public override object VisitPreprocessorIdentifierExpression(AmethystParser.PreprocessorIdentifierExpressionContext context)
    {
        var symbolName = context.IDENTIFIER().GetText();
        var symbol = GetSymbol(symbolName, context);
        
        if (symbol is PreprocessorVariable variable)
        {
            return variable.Value;
        }
        
        throw new SyntaxException($"Unknown symbol type '{symbol.GetType()}' in preprocessor expression. " +
                                  $"Note that stand-alone expressions can trigger this error as well.", context);
    }
}