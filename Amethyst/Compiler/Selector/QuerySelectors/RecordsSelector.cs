using Amethyst.Model;

namespace Amethyst;

public class RecordsSelector : AbstractQuerySelector<AbstractValue>
{
    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        throw new NotImplementedException();
    }
}