using System.Text.RegularExpressions;
using Amethyst.Language;
using Amethyst.Utility;
using static Amethyst.Constants;

namespace Amethyst.Model;

public sealed class SourceFile
{
    public string Name { get; }
    public string Pack { get; }
    public string Namespace { get; }
    public string Registry { get; }
    
    public Scope? Scope { get; set; }
    
    public Dictionary<string, AmethystParser.DeclarationContext> ExportedSymbols { get; } = new();
    public Dictionary<string, string> ImportedSymbols { get; } = new();
    public Dictionary<string, AmethystParser.FunctionDeclarationContext> EntryPointFunctions { get; } = new();

    public readonly Node<string, SourceFile> _tree;

    public SourceFile(string[] pathSegments, bool isInternal = false)
    {
        Pack = pathSegments.Length >= 1 ? pathSegments[0] : throw new ArgumentException("Path must contain a pack name.");
        if (Pack is not DatapackRootDir and not ResourceRootDir)
        {
            throw new ArgumentException($"Pack directory must be either '{DatapackRootDir}' or '{ResourceRootDir}'.");
        }
        Namespace = pathSegments.Length >= 2 ? pathSegments[1] : throw new ArgumentException("Path must contain a namespace.");
        if (!isInternal && Namespace is "minecraft" or InternalNamespaceName || !Regex.IsMatch(Namespace, "^[a-zA-Z0-9_.-]+$"))
        {
            throw new ArgumentException($"Namespace '{Namespace}' is reserved or contains invalid characters.");
        }
        Registry = pathSegments.Length >= 3 ? pathSegments[2] : throw new ArgumentException("Path must contain a registry.");
        if (!Regex.IsMatch(Registry, "^[a-z0-9_]+$"))
        {
            throw new ArgumentException($"Registry '{Registry}' is malformed.");
        }
        
        var subPathSegments = pathSegments[3..];
        Name = subPathSegments.Length > 0 ? subPathSegments[^1] : throw new ArgumentException("Path must contain a name.");
        if (!Regex.IsMatch(Name, "^[a-zA-Z0-9_.-]+$"))
        {
            throw new ArgumentException($"Name '{Name}' contains invalid characters.");
        }
        
        _tree = Node<string, SourceFile>.CreateTree(subPathSegments)!;
    }

    public string GetFullPath()
    {
        return Path.Combine(Pack, Namespace, Registry, GetPath());
    }

    public string GetPath()
    {
        return Path.Combine(_tree.Segments);
    }
    
    public string McFunctionPath => $"{Namespace}:{GetPath().Replace(Path.DirectorySeparatorChar, '/')}";

    public override string ToString()
    {
        return GetPath() + SourceFileExtension;
    }

    internal sealed class GlobalBackup : IDisposable
    {
        private readonly Compiler _owner;
        private readonly SourceFile _previous;
        private readonly Scope _previousScope;
    
        public GlobalBackup(Compiler owner, SourceFile sourceFile)
        {
            _owner = owner;
            _previous = owner.SourceFile;
            _previousScope = owner.Scope;
            owner.SourceFile = sourceFile;
            owner.Scope = sourceFile.Scope!;
        }
    
        public void Dispose()
        {
            _owner.SourceFile = _previous;
            _owner.Scope = _previousScope;
        }
    }
}