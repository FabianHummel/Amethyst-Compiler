using Antlr4.Runtime;

namespace Amethyst;

/// <summary>Exception thrown when a symbol is declared more than once in the same scope.</summary>
public class SymbolAlreadyDeclaredException : SyntaxException
{
    public SymbolAlreadyDeclaredException(string symbolName, ParserRuleContext context) : base(
        $"Symbol '{symbolName}' has already been declared.", context)
    {
    }
}