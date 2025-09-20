using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitCommentStatement(AmethystParser.CommentStatementContext context)
    {
        AddCode($"# {context.STRING_LITERAL().GetText()[1..^1]}");
        return null;
    }
}