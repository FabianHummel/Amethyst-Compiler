namespace Amethyst.Model;

public class Function
{
    public required List<string> Attributes { get; init; }
    public required Scope Scope { get; init; }

    public string McFunctionPath => Scope.McFunctionPath;
    
    public string FilePath => Scope.FilePath;
}