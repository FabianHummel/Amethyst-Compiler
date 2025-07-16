using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public SelectorQueryResult VisitTags_selector(AmethystParser.ExpressionContext context)
    {
        if (VisitExpression(context) is not { } result)
        {
            throw new SyntaxException("Expected expression.", context);
        }

        string queryString;
        bool containsRuntimeValues = false;

        if (result is StaticArrayConstant { BasicType: BasicType.String } arrayConstant)
        {
            if (arrayConstant.Value.Any(constantValue => constantValue is ConstantSubstitute))
            {
                containsRuntimeValues = true;
            }
            
            queryString = string.Join(",", arrayConstant.Value
                .Select(constantValue => $"tag={constantValue.ToTargetSelectorString()}"));
        }
        else if (result is StaticArrayResult { BasicType: BasicType.String } arrayResult)
        {
            var location = arrayResult.NextFreeLocation();
            
            AddCode("data modify storage amethyst:internal data.prefix.prefix set value \"tag=\"");
            AddCode($"data modify storage amethyst:internal data.prefix.in set from storage amethyst: {arrayResult.Location}");
            AddCode("function amethyst:api/data/prefix");
            
            queryString = $"$({location})";

            containsRuntimeValues = true;
        }
        else
        {
            throw new SyntaxException("Expected string array.", context);
        }

        return new SelectorQueryResult("tag", queryString, containsRuntimeValues);
    }
}