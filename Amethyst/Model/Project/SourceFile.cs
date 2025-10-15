using Amethyst.Language;
using Amethyst.Utility;

namespace Amethyst.Model;

public sealed class SourceFile
{
    public string Name { get; }
    public Node<string, SourceFile> Parent { get; }
    public Scope? Scope { get; set; }
    
    public Dictionary<string, AmethystParser.DeclarationContext> ExportedSymbols { get; } = new();
    public Dictionary<string, string> ImportedSymbols { get; } = new();
    public Dictionary<string, AmethystParser.FunctionDeclarationContext> EntryPointFunctions { get; } = new();

    public SourceFile(string name, Node<string, SourceFile> parent)
    {
        Name = name;
        Parent = parent;
    }

    public string GetPath()
    {
        var parentPath = Path.Combine(Parent.Segments[..^1]);
        return Path.Combine(parentPath, Name);
    }

    public override string ToString()
    {
        return GetPath() + Constants.SourceFileExtension;
    }
}

internal sealed class DisposableSourceFile : IDisposable
{
    private readonly Compiler _owner;
    private readonly SourceFile _previous;
    private readonly DisposableScope _scope;
    
    public DisposableSourceFile(Compiler owner, SourceFile sourceFile, Scope scope)
    {
        _owner = owner;
        _previous = owner.SourceFile;
        owner.SourceFile = sourceFile;
        _scope = new DisposableScope(owner, scope);
    }
    
    public void Dispose()
    {
        _owner.SourceFile = _previous;
        _scope.Dispose();
    }
}