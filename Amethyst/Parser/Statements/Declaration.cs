using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt Declaration()
    {
        if (Match(TokenType.NAMESPACE))
        {
            return NsDeclaration();
        }

        if (Check(TokenType.FUNCTION))
        {
            return FuncDeclaration();
        }

        return Statement();
    }
}