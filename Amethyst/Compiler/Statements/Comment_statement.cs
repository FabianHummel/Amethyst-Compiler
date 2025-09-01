using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitComment_statement(AmethystParser.Comment_statementContext context)
    {
        AddCode($"# {context.STRING_LITERAL().GetText()[1..^1]}");
        return null;
    }
}