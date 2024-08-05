using Amethyst.Language;

namespace Amethyst.Model;

public class SourceFile
{
    public required AmethystParser.FileContext Context { get; init; }
    public required string Path { get; init; }
}