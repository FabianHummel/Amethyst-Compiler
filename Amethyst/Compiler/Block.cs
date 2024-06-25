using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitBlock(AmethystParser.BlockContext context)
    {
        var previousScope = Scope;
        Scope = new Scope
        {
            Name = null,
            Parent = previousScope,
            Context = Context
        };
        foreach (var statementContext in context.statement())
        {
            VisitStatement(statementContext);
        }
        Scope = previousScope;
        return null;
    }
}