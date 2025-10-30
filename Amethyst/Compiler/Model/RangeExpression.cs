namespace Amethyst.Model;

public class RangeExpression
{
    public AbstractValue? Start { get; set; }
    public AbstractValue? Stop { get; set; }
        
    public bool ContainsRuntimeValues => Start is IRuntimeValue || Stop is IRuntimeValue;

    public override string ToString()
    {
        var start = Start?.ToTargetSelectorString();
        var stop = Stop?.ToTargetSelectorString();

        return $"{start}..{stop}";
    }
}