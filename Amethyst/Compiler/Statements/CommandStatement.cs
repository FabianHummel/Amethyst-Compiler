using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitCommandStatement(AmethystParser.CommandStatementContext context)
    {
        AddCode(context.COMMAND().GetText()[1..]);
        return null;
    }
}