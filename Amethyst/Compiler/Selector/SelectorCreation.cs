using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Parses a full target selector. First, it parses the selector queries and assembles the
    ///     selector string. Subsequently, the selector is either simply created when possible or invoked
    ///     with a macro when using runtime variables. Additionally, the query keys are reordered in the
    ///     most optimal way possible in <see cref="ProcessQueryDict" />.</p>
    ///     <p><inheritdoc /></p>
    /// </summary>
    public override AbstractValue VisitSelectorCreation(AmethystParser.SelectorCreationContext context)
    {
        var entitySelectorTypeContext = context.entitySelectorType();
        var entityTargetResult = VisitEntitySelectorType(entitySelectorTypeContext);

        var sp = StackPointer;

        var selectorElementContexts = context.selectorElement();
        var queryDict = new Dictionary<string, List<SelectorQueryValue>>(); 
        ProcessSelectorElements(selectorElementContexts, ref queryDict, out var containsRuntimeValues, out var limitExpression);

        var finalQueryDict = new Dictionary<string, SelectorQueryResult>();
        foreach (var (queryKey, queryValues) in queryDict)
        {
            if (!Constants.TargetSelectorQueryKeys.TryGetValue(queryKey, out var selector))
            {
                ConsoleUtility.PrintWarning($"Unknown target selector query key '{queryKey}'. Parsing result as-is.");
                continue;
            }
            
            finalQueryDict[queryKey] = selector.Transform(queryKey, queryValues.ToArray());
        }
        
        var selectorString = $"{entityTargetResult.Target.GetMcfOperatorSymbol()}{ProcessQueryDict(finalQueryDict)}";

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

    /// <summary>Processes a collection of selector queries and builds the <paramref name="queryDict" />.</summary>
    /// <param name="selectorElementContexts">The target selector queries.</param>
    /// <param name="queryDict">The resulting map of query keys and values used in the selector query.</param>
    /// <param name="containsRuntimeValues">Whether the selector query contains values that are only known
    /// during runtime. If this is the case, the selector needs to be created using a macro invocation.</param>
    /// <param name="limitExpression">The value of the limit query if used in the target selector query.
    /// This is used to determine if the selector only matches a single or multiple entities.</param>
    private void ProcessSelectorElements(IEnumerable<AmethystParser.SelectorElementContext> selectorElementContexts, ref Dictionary<string, List<SelectorQueryValue>> queryDict, out bool containsRuntimeValues, out AbstractValue? limitExpression)
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
                
                containsRuntimeValues |= queryResult.ContainsRuntimeValues;
                limitExpression ??= queryResult.LimitExpression;

                if (!queryDict.TryGetValue(queryResult.QueryKey, out var selectorQueryResult))
                {
                    queryDict.Add(queryResult.QueryKey, new List<SelectorQueryValue>(queryResult.QueryValues));
                }
                else
                {
                    selectorQueryResult.AddRange(queryResult.QueryValues);
                }
            }
        }
    }

    /// <summary>Creates the query selector and stores the resulting entities in a variable.</summary>
    /// <param name="selector">The base selector.</param>
    /// <param name="limitExpression">The value of the limit query if used in the target selector query.
    /// This is used to determine if the selector only matches a single or multiple entities.</param>
    /// <param name="selectorString">The final query string that is used with the
    /// <paramref name="selector" />.</param>
    /// <param name="context">The parser rule context that is given to the resulting variable.</param>
    /// <param name="isMacroInvocation">Whether the selector needs to be created using a macro invocation.</param>
    /// <returns>A value that contains the matches entities of the target selector.</returns>
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

    /// <summary>Processes a list of selector queries and reorders them in the most optimal way. The most
    /// optimal order is a statistical evaluation of which queries filter out the most entities when used.</summary>
    /// <param name="queryList">The queries to process.</param>
    /// <returns>The final query string.</returns>
    private static string? ProcessQueryDict(Dictionary<string, SelectorQueryResult> queryList)
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
            if (queryList.TryGetValue(queryKey, out var queryResult))
            {
                queryString.Add(queryResult.ToTargetSelectorString());
            }
        }

        return $"[{string.Join(",", queryString)}]";
    }
}