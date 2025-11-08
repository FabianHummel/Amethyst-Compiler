namespace Amethyst.Utility;

public static class ParserUtility
{
    /// <summary>Flattens deeply nested parser rules into a single list. This is done by recursively
    /// retrieving the child rules, filtering them by their type and yielding back to the final result.</summary>
    /// <param name="ctx">The context that contains the child rules.</param>
    /// <param name="getChildren">A function to retrieve the actual parser rules on the passed context.</param>
    /// <typeparam name="TTarget">The type of the resulting elements.</typeparam>
    /// <typeparam name="TCheck">The type the context must be of. This is used to filter the child rules to
    /// the required subset.</typeparam>
    /// <returns></returns>
    public static IEnumerable<TTarget> FlattenParserRules<TTarget, TCheck>(TTarget ctx, Func<TCheck, TTarget[]> getChildren) 
        where TCheck : TTarget
    {
        if (ctx is TCheck targetContext)
        {
            var children = getChildren(targetContext);
            foreach (var e in FlattenParserRules(children[0], getChildren))
            {
                yield return e;
            }
            foreach (var e in FlattenParserRules(children[1], getChildren))
            {
                yield return e;
            }
        }
        else yield return ctx;
    }
}