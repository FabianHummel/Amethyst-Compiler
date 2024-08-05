using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitComment_statement(AmethystParser.Comment_statementContext context)
    {
        AddCode($"# {context.String_Literal().Symbol}");
        return null;
    }
}