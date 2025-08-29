namespace Amethyst.Model;

public class SourceFolder
{
    public required Context Context { get; init; }
    public Dictionary<string, SourceFolder> Children { get; } = new();
    public Dictionary<string, SourceFile> SourceFiles { get; } = new();

    public SourceFolder CreateOrGetFolderForPath(string parserFilePath)
    {
        var parts = parserFilePath.Split(Path.DirectorySeparatorChar);
        if (parts.Length == 1) return this;

        var current = this;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (!Children.TryGetValue(parts[i], out var folder))
            {
                folder = new SourceFolder
                {
                    Context = Context,
                };
                current.Children.Add(parts[i], folder);
            }
            current = folder;
        }
        return current;
    }
}