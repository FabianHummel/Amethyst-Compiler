using Amethyst.Language;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public class RangeExpressionResult
    {
        private readonly AmethystParser.RangeExpressionContext _context;
        private readonly bool _allowDecimals;

        public RangeExpressionResult(AmethystParser.RangeExpressionContext context, bool allowDecimals)
        {
            _context = context;
            _allowDecimals = allowDecimals;
        }

        public AbstractValue? Start { get; set; }
        public AbstractValue? Stop { get; set; }
        
        public object? OverwrittenStartValue { get; set; }
        
        public ParserRuleContext Context => _context;
        
        public bool ContainsRuntimeValues => Start is IRuntimeValue || Stop is IRuntimeValue;

        public string ToTargetSelectorString()
        {
            if (Start is null && Stop is null && OverwrittenStartValue is null)
            {
                throw new SyntaxException("Invalid range expression. At least one of the range bounds must be specified.", _context);
            }
            
            if (!_allowDecimals && (Start is ConstantDecimal or RuntimeDecimal || Stop is ConstantDecimal or RuntimeDecimal))
            {
                throw new SyntaxException("Unexpected decimal value.", _context);
            }

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
        var result = new RangeExpressionResult(context, allowDecimals);

        if (context.GetChild(0) is AmethystParser.ExpressionContext startExpressionContext)
        {
            result.Start = VisitExpression(startExpressionContext);
        }
        
        if (context.GetChild(context.ChildCount - 1) is AmethystParser.ExpressionContext stopExpressionContext)
        {
            result.Stop = VisitExpression(stopExpressionContext);
        }

        return result;
    }
}