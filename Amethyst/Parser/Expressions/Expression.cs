using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Expr Expression()
    {
        return Assignment();
    }
}