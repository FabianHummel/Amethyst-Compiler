using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitMemberExpression(AmethystParser.MemberExpressionContext context)
    {
        var expressionContext = context.expression();
        var result = VisitExpression(expressionContext);
        
        if (result is not IMemberAccess memberAccess)
        {
            throw new SyntaxException($"Type '{result.Datatype}' does not support accessing members.", expressionContext);
        }

        var identifier = context.IDENTIFIER();
        var memberName = identifier.GetText();

        if (memberAccess.GetMember(memberName) is not { } member)
        {
            throw new SyntaxException($"No member '{memberName}' found on type '{result.Datatype}'.", context);
        }

        return member;
    }
}