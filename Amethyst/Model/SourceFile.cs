using Amethyst.Language;

namespace Amethyst.Model;

public class SourceFile
{
    public required string Path { get; init; }
    public required string Name { get; init; }
    public required Scope RootScope { get; init; }
    public Dictionary<string, AmethystParser.DeclarationContext> ExportedSymbols { get; } = new();
    public Dictionary<string, string> ImportedSymbols { get; } = new();
    public Dictionary<string, AmethystParser.Function_declarationContext> EntryPointFunctions { get; } = new();
}