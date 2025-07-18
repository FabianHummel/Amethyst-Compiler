using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitEquality_expression(AmethystParser.Equality_expressionContext context)
    {
        if (context.comparison_expression() is not { } comparisonExpressionContexts)
        {
            throw new UnreachableException();
        }

        if (comparisonExpressionContexts.Length == 1)
        {
            return VisitComparison_expression(comparisonExpressionContexts[0]);
        }

        if (VisitComparison_expression(comparisonExpressionContexts[0]) is not { } previous)
        {
            throw new SyntaxException("Expected comparison expression.", comparisonExpressionContexts[0]);
        }

        for (int i = 1; i < comparisonExpressionContexts.Length; i++)
        {
            if (VisitComparison_expression(comparisonExpressionContexts[i]) is not { } current)
            {
                throw new SyntaxException("Expected comparison expression.", comparisonExpressionContexts[i]);
            }

            // check data type
            if (!previous.DataType.Equals(current.DataType))
            {
                throw new SyntaxException(
                    $"Cannot compare values of type '{current.DataType}' with '{previous.DataType}'.",
                    comparisonExpressionContexts[i]);
            }

            if (previous is ConstantValue lhsConstant && current is ConstantValue rhsConstant)
            {
                var result = lhsConstant.Equals(rhsConstant);

                previous = new BooleanConstant
                {
                    Compiler = this,
                    Context = comparisonExpressionContexts[i],
                    Value = result
                };

                continue;
            }

            int location;
            
            if (previous is RuntimeValue lhsRuntime && current is RuntimeValue rhsRuntime)
            {
                var backup = lhsRuntime.EnsureBackedUp();
                
                AddCode($"execute store success score {backup.Location} amethyst run data modify storage amethyst: {backup.Location} set from storage amethyst: {rhsRuntime.Location}");
                AddCode($"execute store success score {backup.Location} amethyst if score {backup.Location} amethyst matches 0");

                location = backup.Location;
            }
            else
            {
                // switch operands so that the constant value is always on the right side for optimization
                var (lhs, rhs) = AbstractResult.EnsureConstantValueIsLast(previous, current);
            
                location = lhs.NextFreeLocation();
                
                if (lhs.DataType.Location == DataLocation.Scoreboard && rhs is INumericConstant numericConstant)
                {
                    // if both sides are a decimal type, coerce the right side to the left side's decimal places.
                    // this allows syntax like this, where the number of decimal places don't exactly match:
                    // 1 | var x = 0.1234;
                    // 2 | if (x == 1.0) { }
                    // 3 | if (x == 0.12345678) { }
                    if (lhs is DecimalResult lhsDecimal && numericConstant is DecimalConstant rhsDecimal)
                    {
                        numericConstant = new DecimalConstant
                        {
                            Compiler = rhsDecimal.Compiler,
                            Context = rhsDecimal.Context,
                            Value = rhsDecimal.Value,
                            DecimalPlaces = lhsDecimal.DecimalPlaces
                        };
                    }
                
                    AddCode($"execute store success score {location} amethyst if score {lhs.Location} amethyst matches {numericConstant.ScoreboardValue}");
                }
                else
                {
                    AddCode($"execute store success score {location} amethyst if data storage amethyst: {{{lhs.Location}:{rhs.ToNbtString()}}}");
                }
            }
            
            previous = new BooleanResult
            {
                Compiler = this,
                Context = comparisonExpressionContexts[i],
                Location = location,
                IsTemporary = true
            };
        }

        return previous;
    }
}