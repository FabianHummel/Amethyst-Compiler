namespace Amethyst.Model;

public class Function : Symbol
{
    public required HashSet<string> Attributes { get; init; }
    public required Variable[] Parameters { get; init; }
    public required Scope Scope { get; init; }
}