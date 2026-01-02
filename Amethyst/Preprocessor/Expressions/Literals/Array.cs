using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override IPreprocessorValue[] VisitPreprocessorArrayLiteral(AmethystParser.PreprocessorArrayLiteralContext context)
    {
        var arrayElementContexts = context.preprocessorArrayElement();
        var elements = new List<IPreprocessorValue>();
        ProcessArrayElements(arrayElementContexts, ref elements);
        return elements.ToArray();
    }

    private void ProcessArrayElements(IEnumerable<AmethystParser.PreprocessorArrayElementContext> arrayElementContexts, ref List<IPreprocessorValue> elements)
    {
        foreach (var arrayElementContext in arrayElementContexts)
        {
            if (arrayElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                var result = VisitPreprocessorYieldingStatement<IPreprocessorValue>(preprocessorYieldingStatementContext);
                elements.AddRange(result);
            }
            else if (arrayElementContext.preprocessorExpression() is { } expressionContext)
            {
                elements.Add(VisitPreprocessorExpression(expressionContext));
            }
        }
    }
}