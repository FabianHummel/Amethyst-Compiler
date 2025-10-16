using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitCommentStatement(AmethystParser.CommentStatementContext context)
    {
        this.AddCode($"# {context.STRING_LITERAL().GetText()[1..^1]}");
        return null;
    }
}