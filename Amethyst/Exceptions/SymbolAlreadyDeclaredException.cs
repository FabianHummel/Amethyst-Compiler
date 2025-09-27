using Antlr4.Runtime;

namespace Amethyst;

public class SymbolAlreadyDeclaredException : SyntaxException
{
    public SymbolAlreadyDeclaredException(string symbolName, ParserRuleContext context) : base(
        $"Symbol '{symbolName}' has already been declared.", context)
    {
    }
}