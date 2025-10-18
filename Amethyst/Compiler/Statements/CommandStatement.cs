using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitCommandStatement(AmethystParser.CommandStatementContext context)
    {
        this.AddCode(context.COMMAND().GetText().TrimStart()[1..]);
        return null;
    }
}