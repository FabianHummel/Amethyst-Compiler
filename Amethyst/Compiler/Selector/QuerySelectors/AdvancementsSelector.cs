using Amethyst.Model;

namespace Amethyst;

/// <inheritdoc />
/// <summary><p>Parses a collection of advancements.</p><p><inheritdoc /></p></summary>
public class AdvancementsSelector : AbstractQuerySelector<AbstractValue>
{
    public override SelectorQueryResult Parse(string queryKey, bool isNegated, AbstractValue value)
    {
        throw new NotImplementedException();
    }
}