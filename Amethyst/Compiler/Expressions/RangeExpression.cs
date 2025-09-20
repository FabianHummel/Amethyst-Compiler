using Amethyst.Language;

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

        public AbstractResult? Start { get; set; }
        public AbstractResult? Stop { get; set; }
        
        public bool ContainsRuntimeValues => Start is RuntimeValue || Stop is RuntimeValue;

        public override string ToString()
        {
            if (Start is null && Stop is null)
            {
                throw new SyntaxException("Invalid range expression.", _context);
            }
            
            if (!_allowDecimals && (Start is DecimalConstant or DecimalResult || Stop is DecimalConstant or DecimalResult))
            {
                throw new SyntaxException("Unexpected decimal value.", _context);
            }
            
            var start = Start?.ToTargetSelectorString();
            var stop = Stop?.ToTargetSelectorString();

            return $"{start}..{stop}";
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