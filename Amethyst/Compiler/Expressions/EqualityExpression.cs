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

        if (left.Datatype != right.Datatype)
        {
            throw new SyntaxException($"Cannot compare values of type '{left.Datatype}' and '{right.Datatype}'.", context);
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

        Location location;
        
        var mcfComparisonOp = op switch
        {
            ComparisonOperator.EQUAL => "if",
            ComparisonOperator.NOT_EQUAL => "unless",
            _ => throw new SyntaxException($"Invalid equality operator '{op}'.", context)
        };
        
        if (left is IRuntimeValue lhsRuntime && right is IRuntimeValue rhsRuntime)
        {
            var backup = lhsRuntime.EnsureBackedUp();
            
            this.AddCode($"execute store success score {backup.Location} run data modify storage {backup.Location} set from storage {rhsRuntime.Location}");
            this.AddCode($"execute store success score {backup.Location} {mcfComparisonOp} score {backup.Location} matches 0");

            location = backup.Location;
        }
        else
        {
            // switch operands so that the constant value is always on the right side for optimization
            var (lhs, rhs) = AbstractValue.EnsureConstantValueIsLast(left, right);
        
            location = lhs.NextFreeLocation(DataLocation.Scoreboard);
            
            if (lhs.Location.DataLocation == DataLocation.Scoreboard && rhs is IScoreboardValue numericConstant)
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
            
                this.AddCode($"execute store success score {location} {mcfComparisonOp} score {lhs.Location} matches {numericConstant.ScoreboardValue}");
            }
            else
            {
                this.AddCode($"execute store success score {location} {mcfComparisonOp} data storage {{{lhs.Location}:{rhs.ToNbtString()}}}");
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