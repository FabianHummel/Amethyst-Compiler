namespace Amethyst.Utility;

public static class ParserUtility
{
    public static IEnumerable<TParent> FlattenParserRules<TParent, TTarget>(TParent ctx, Func<TTarget, TParent[]> getChildren) 
        where TTarget : TParent
    {
        if (ctx is TTarget targetContext)
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