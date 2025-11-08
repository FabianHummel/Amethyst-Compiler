using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Accesses a member of any value that supports accessing its members. (implementing
    ///     <see cref="IMemberAccess" />).</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">The target does not support accessing its members.</exception>
    /// <exception cref="SyntaxException">The requested member was not found on the target.</exception>
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