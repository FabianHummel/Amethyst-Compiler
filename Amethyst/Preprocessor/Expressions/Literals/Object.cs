using Amethyst.Language;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override Dictionary<string, IPreprocessorValue> VisitPreprocessorObjectLiteral(AmethystParser.PreprocessorObjectLiteralContext context)
    {
        var objectElementContexts = context.preprocessorObjectElement();
        var elements = new Dictionary<string, IPreprocessorValue>();
        ProcessObjectElements(objectElementContexts, ref elements);
        return elements;
    }

    public override KeyValuePairClass<PreprocessorString, AbstractPreprocessorValue> VisitPreprocessorObjectKvp(AmethystParser.PreprocessorObjectKvpContext context)
    {
        var key = (PreprocessorString)Visit(context.preprocessorStringLiteral())!;
        var result = VisitPreprocessorExpression(context.preprocessorExpression());
        return new KeyValuePairClass<PreprocessorString, AbstractPreprocessorValue>(key, result);
    }

    private void ProcessObjectElements(IEnumerable<AmethystParser.PreprocessorObjectElementContext> objectElementContexts, ref Dictionary<string, IPreprocessorValue> elements)
    {
        foreach (var objectElementContext in objectElementContexts)
        {
            if (objectElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                foreach (var kvp in VisitPreprocessorYieldingStatement<KeyValuePairClass<PreprocessorString, AbstractPreprocessorValue>>(preprocessorYieldingStatementContext))
                {
                    ProcessObjectElement(kvp, elements, objectElementContext);
                }
            }
            else if (objectElementContext.preprocessorObjectKvp() is { } objectKvpContext)
            {
                var kvp = VisitPreprocessorObjectKvp(objectKvpContext);

                ProcessObjectElement(kvp, elements, objectElementContext);
            }
        }
    }

    private static void ProcessObjectElement(KeyValuePairClass<PreprocessorString, AbstractPreprocessorValue> kvp, Dictionary<string, IPreprocessorValue> elements, AmethystParser.PreprocessorObjectElementContext objectElementContext)
    {
        if (elements.Keys.Any(k => k == kvp.Key.Value))
        {
            throw new SyntaxException($"Duplicate key '{kvp.Key}'.", objectElementContext);
        }

        elements.Add(kvp.Key.Value, kvp.Value);
    }
}