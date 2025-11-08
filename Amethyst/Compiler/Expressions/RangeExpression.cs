using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public class RangeExpressionResult
    {
        private readonly AmethystParser.RangeExpressionContext _context;

        public RangeExpressionResult(AmethystParser.RangeExpressionContext context)
        {
            _context = context;
        }

        public AbstractValue? Start { get; set; }
        public AbstractValue? Stop { get; set; }
        
        public object? OverwrittenStartValue { get; set; }
        
        public ParserRuleContext Context => _context;
        
        public bool ContainsRuntimeValues => Start is IRuntimeValue || Stop is IRuntimeValue;

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