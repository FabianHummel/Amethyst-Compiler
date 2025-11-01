using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitSelectorCreation(AmethystParser.SelectorCreationContext context)
    {
        var entitySelectorTypeContext = context.entitySelectorType();
        var entityTargetResult = VisitEntitySelectorType(entitySelectorTypeContext);

        var sp = StackPointer;

        var selectorElementContexts = context.selectorElement();
        var queryDict = new Dictionary<string, string>(); 
        ProcessSelectorElements(selectorElementContexts, ref queryDict, out var containsRuntimeValues, out var limitExpression);
        
        var selectorString = $"{entityTargetResult.Target.GetMcfOperatorSymbol()}{ProcessQueryDict(queryDict)}";

        StackPointer = sp;

        if (!containsRuntimeValues)
        {
            return CreateSelector(entityTargetResult, limitExpression, selectorString, context);
        }

        string mcFunctionPath;
        AbstractValue value;
        using (this.EvaluateScoped("_create_selector"))
        {
            mcFunctionPath = Scope.McFunctionPath;
            value = CreateSelector(entityTargetResult, limitExpression, selectorString, context, isMacroInvocation: true);
        }
            
        this.AddCode($"function {mcFunctionPath} with storage amethyst:");

        return value;

    }
    
    private void ProcessSelectorElements(IEnumerable<AmethystParser.SelectorElementContext> selectorElementContexts, ref Dictionary<string, string> queryDict, out bool containsRuntimeValues, out AbstractValue? limitExpression)
    {
        containsRuntimeValues = false;
        limitExpression = null;
        
        foreach (var selectorElementContext in selectorElementContexts)
        {
            if (selectorElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                var result = VisitPreprocessorYieldingStatement<AmethystParser.SelectorElementContext>(preprocessorYieldingStatementContext);
                ProcessSelectorElements(result, ref queryDict, out containsRuntimeValues, out limitExpression);
            }
            else if (selectorElementContext.selectorQuery() is { } selectorQueryContext)
            {
                var queryResult = VisitSelectorQuery(selectorQueryContext);

                if (queryDict.ContainsKey(queryResult.QueryKey))
                {
                    throw new SyntaxException($"Duplicate selector '{queryResult.QueryKey}'.", selectorQueryContext);
                }
                
                containsRuntimeValues |= queryResult.ContainsRuntimeValues;
                limitExpression ??= queryResult.LimitExpression;
                queryDict.Add(queryResult.QueryKey, queryResult.QueryValue);
            }
        }
    }
    
    private AbstractValue CreateSelector(EntityTargetResult selector, AbstractValue? limitExpression, string? selectorString, AmethystParser.SelectorCreationContext context, bool isMacroInvocation = false)
    {
        var prefix = isMacroInvocation ? "$" : string.Empty;
            
        var location = Location.Storage(++StackPointer);

        if (selector.IsSingleTarget(limitExpression))
        {
            this.AddCode($"{prefix}execute as {selectorString} run function amethyst:libraries/gu/generate");
            this.AddCode($"data modify storage {location} set from storage gu:main out");
            
            return new RuntimeEntity
            {
                Compiler = this,
                Context = context,
                Location = location,
                IsTemporary = true
            };
        }
        
        this.AddCode($"data modify storage {location} set value []");

        string mcFunctionPath;
        using (this.EvaluateScoped("_multi_selector"))
        {
            mcFunctionPath = Scope.McFunctionPath;
            this.AddCode("function amethyst:libraries/gu/generate");
            this.AddCode($"data modify storage {location} append from storage gu:main out");
        }

        this.AddCode($"{prefix}execute as {selectorString} run function {mcFunctionPath}");

        return new RuntimeStaticArray
        {
            Compiler = this,
            Context = context,
            BasicType = BasicType.Entity,
            Location = location,
            IsTemporary = true
        };
    }
    
    private static string? ProcessQueryDict(IReadOnlyDictionary<string, string> queryList)
    {
        if (queryList.Count == 0)
        {
            return null;
        }

        var optimalQueryOrder = new[]
        {
            "x", "y", "z", "dx", "dy", "dz",
            "type", "level", "gamemode", "team",
            "tag", "name", "records", "distance",
            "x_rotation", "y_rotation", "advancement",
            "predicate", "nbt", "limit", "sort"
        };
        
        var queryString = new List<string>();
        
        foreach (var queryKey in optimalQueryOrder.Concat(queryList.Keys.Except(optimalQueryOrder)))
        {
            if (queryList.TryGetValue(queryKey, out var queryValue))
            {
                queryString.Add(queryValue);
            }
        }

        return $"[{string.Join(",", queryString)}]";
    }
}