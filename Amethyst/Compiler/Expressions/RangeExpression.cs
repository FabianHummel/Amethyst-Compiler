using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>A range expression result holds a lower and an upper numeric bound that is primarily used
    /// in target selectors.</summary>
    public class RangeExpressionResult
    {
        /// <summary>The parser rule context that holds the original information about the range expression
        /// creation.</summary>
        private readonly AmethystParser.RangeExpressionContext _context;

        /// <summary>Creates a new instance of <see cref="RangeExpressionResult" />.</summary>
        /// <param name="context">The original parser rule context used for error handling.</param>
        public RangeExpressionResult(AmethystParser.RangeExpressionContext context)
        {
            _context = context;
        }

        /// <summary>The lower bound. If null, this bound is unbounded and usually reaches to negative
        /// infinity.</summary>
        public AbstractValue? Start { get; set; }

        /// <summary>The upper bound. If null, this bound is unbounded and usually reaches to positive
        /// infinity.</summary>
        public AbstractValue? Stop { get; set; }

        /// <summary>Optionally override the start value with a fixed value. This is used in special logic
        /// where the lower bound should be in fact zero.</summary>
        public object? OverwrittenStartValue { get; set; }

        /// <inheritdoc cref="_context" />
        public ParserRuleContext Context => _context;

        /// <summary>Whether this range contains values that are only known at runtime.</summary>
        public bool ContainsRuntimeValues => Start is IRuntimeValue || Stop is IRuntimeValue;

        /// <summary>Converts this range expression to a string ready to be used in target selectors. The
        /// format is: <c>&lt;lb&gt;..&lt;ub&gt;</c></summary>
        /// <returns></returns>
        public string ToTargetSelectorString()
        {
            var start = Start?.ToTargetSelectorString() ?? OverwrittenStartValue?.ToString() ?? "";
            var stop = Stop?.ToTargetSelectorString() ?? "";

            return $"{start}..{stop}";
        }
        
        public override string ToString()
        {
            return ToTargetSelectorString();
        }
    }

    /// <inheritdoc cref="AmethystParserBaseVisitor{Result}.VisitRangeExpression" />
    /// <summary>
    ///     <p>Creates a <see cref="RangeExpressionResult" /> and optionally allows decimal values as well.
    ///     If not, they are filtered out and an exception is raised.</p>
    ///     <p><inheritdoc cref="AmethystParserBaseVisitor{Result}.VisitRangeExpression" /></p></summary>
    /// <param name="allowDecimals">Whether to allow decimal values in the range expression.</param>
    /// <exception cref="SyntaxException">The range expression is invalid by specifying no bounds at all or
    /// using decimal values when it's not allowed.</exception>
    public RangeExpressionResult VisitRangeExpression(AmethystParser.RangeExpressionContext context, bool allowDecimals)
    {
        var result = new RangeExpressionResult(context);

        if (context.GetChild(0) is AmethystParser.ExpressionContext startExpressionContext)
        {
            result.Start = VisitExpression(startExpressionContext);
        }
        
        if (context.GetChild(context.ChildCount - 1) is AmethystParser.ExpressionContext stopExpressionContext)
        {
            result.Stop = VisitExpression(stopExpressionContext);
        }
        
        if (result.Start is null && result.Stop is null)
        {
            throw new SyntaxException("Invalid range expression. At least one of the range bounds must be specified.", context);
        }
            
        if (!allowDecimals && (result.Start is ConstantDecimal or RuntimeDecimal || result.Stop is ConstantDecimal or RuntimeDecimal))
        {
            throw new SyntaxException("Unexpected decimal value.", context);
        }

        return result;
    }
}