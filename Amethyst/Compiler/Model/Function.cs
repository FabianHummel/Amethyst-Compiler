namespace Amethyst.Model;

public class Function : Symbol
{
    public required HashSet<string> Attributes { get; init; }
}