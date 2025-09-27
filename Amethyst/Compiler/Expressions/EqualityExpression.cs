using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitEqualityExpression(AmethystParser.EqualityExpressionContext context)
    {
        var expressionContexts = context.expression();
        var left = VisitExpression(expressionContexts[0]);
        var right = VisitExpression(expressionContexts[1]);
        var operatorToken = context.GetChild(1).GetText();
        
        var op = Enum.GetValues<ComparisonOperator>()
            .First(op => op.GetAmethystOperatorSymbol() == operatorToken);

        if (left.DataType != right.DataType)
        {
            throw new SyntaxException($"Cannot compare values of type '{left.DataType}' and '{right.DataType}'.", context);
        }

        if (left is IConstantValue lhsConstant && right is IConstantValue rhsConstant)
        {
            var result = lhsConstant.Equals(rhsConstant);

            return new ConstantBoolean
            {
                Compiler = this,
                Context = context,
                Value = result
            };
        }

        int location;
        
        var mcfComparisonOp = op switch
        {
            ComparisonOperator.EQUAL => "if",
            ComparisonOperator.NOT_EQUAL => "unless",
            _ => throw new SyntaxException($"Invalid equality operator '{op}'.", context)
        };
        
        if (left is IRuntimeValue lhsRuntime && right is IRuntimeValue rhsRuntime)
        {
            var backup = lhsRuntime.EnsureBackedUp();
            
            AddCode($"execute store success score {backup.Location} amethyst run data modify storage amethyst: {backup.Location} set from storage amethyst: {rhsRuntime.Location}");
            AddCode($"execute store success score {backup.Location} amethyst {mcfComparisonOp} score {backup.Location} amethyst matches 0");

            location = backup.Location;
        }
        else
        {
            // switch operands so that the constant value is always on the right side for optimization
            var (lhs, rhs) = AbstractValue.EnsureConstantValueIsLast(left, right);
        
            location = lhs.NextFreeLocation();
            
            if (lhs.DataType.Location == DataLocation.Scoreboard && rhs is IScoreboardValue numericConstant)
            {
                // if both sides are a decimal type, coerce the right side to the left side's decimal places.
                // this allows syntax like this, where the number of decimal places don't exactly match:
                //  1 | var x = 0.1234;
                //  2 | if (x == 1.0) { }
                //  3 | if (x == 0.12345678) { }
                if (lhs is RuntimeDecimal lhsDecimal && numericConstant is ConstantDecimal rhsDecimal)
                {
                    numericConstant = new ConstantDecimal
                    {
                        Compiler = rhsDecimal.Compiler,
                        Context = rhsDecimal.Context,
                        Value = rhsDecimal.Value,
                        DecimalPlaces = lhsDecimal.DecimalPlaces
                    };
                }
            
                AddCode($"execute store success score {location} amethyst {mcfComparisonOp} score {lhs.Location} amethyst matches {numericConstant.ScoreboardValue}");
            }
            else
            {
                AddCode($"execute store success score {location} amethyst {mcfComparisonOp} data storage amethyst: {{{lhs.Location}:{rhs.ToNbtString()}}}");
            }
        }
        
        return new RuntimeBoolean
        {
            Compiler = this,
            Context = context,
            Location = location,
            IsTemporary = true
        };
    }
}