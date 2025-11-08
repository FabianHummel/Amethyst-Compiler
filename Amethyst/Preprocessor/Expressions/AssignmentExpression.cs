using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <summary><p>Assigns a value to a preprocessor variable.</p><p><inheritdoc /></p></summary>
    /// <exception cref="SyntaxException">Thrown when an invalid operator is used.</exception>
    /// <seealso cref="VisitAssignmentExpression" />
    public override object? VisitPreprocessorAssignmentExpression(AmethystParser.PreprocessorAssignmentExpressionContext context)
    {
        var expressionContexts = context.preprocessorExpression();
        var lhs = VisitPreprocessorExpression(expressionContexts[0]);
        var rhs = VisitPreprocessorExpression(expressionContexts[1]);
        
        var operatorToken = context.GetChild(1).GetText();
        var op = Enum.GetValues<AssignmentOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        if (op == AssignmentOperator.ASSIGN)
        {
            AssignPreprocessorValue(lhs, rhs);
            return null;
        }
        
        var result = op switch
        {
            AssignmentOperator.ADD => lhs + rhs,
            AssignmentOperator.SUBTRACT => lhs - rhs,
            AssignmentOperator.MULTIPLY => lhs * rhs,
            AssignmentOperator.DIVIDE => lhs / rhs,
            AssignmentOperator.MODULO => lhs % rhs,
            _ => throw new SyntaxException($"Invalid operator '{op}'.", context)
        };
        
        AssignPreprocessorValue(lhs, result);
        return null;
        
        void AssignPreprocessorValue(AbstractPreprocessorValue target, AbstractPreprocessorValue source)
        {
            target.SetValue(source.AbstractValue);
        }
    }
}