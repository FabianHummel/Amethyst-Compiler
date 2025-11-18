using System.Text.RegularExpressions;
using Amethyst.Language;
using Amethyst.Utility;
using static Amethyst.Model.Constants;

namespace Amethyst.Model;

/// <summary>Represents a source file in the Amethyst project.</summary>
public sealed class SourceFile
{
    /// <summary>The name of the source file.</summary>
    public string Name { get; }

    /// <summary>The pack of the source file (either "datapack" or "resourcepack").</summary>
    public string Pack { get; }

    /// <summary>The namespace of the source file.</summary>
    public string Namespace { get; }

    /// <summary>The registry of the source file.</summary>
    public string Registry { get; }

    /// <summary>The root scope associated with this source file.</summary>
    public Scope? Scope { get; set; }

    public AmethystParser.FileContext? Ast { get; set; }

    /// <summary>A dictionary of symbols exported by this source file. These can be imported by other
    /// source files using import statements.</summary>
    /// <seealso cref="Compiler.VisitPreprocessorFromImportDeclaration" />
    public Dictionary<string, AmethystParser.DeclarationContext> ExportedSymbols { get; } = new();

    /// <summary>A dictionary of already visited declarations so the same declaration isn't processed
    /// twice.</summary>
    public Dictionary<AmethystParser.DeclarationContext, Symbol> DeclarationCache { get; } = new();

    /// <summary>A dictionary of symbols imported by this source file from other source files using import
    /// statements.</summary>
    /// <seealso cref="Compiler.VisitPreprocessorFromImportDeclaration" />
    /// <seealso cref="Compiler.VisitPreprocessorImportAsDeclaration" />
    public Dictionary<string, string> ImportedSymbols { get; } = new();

    /// <summary>A dictionary of entry point functions defined in this source file. When the project is
    /// compiled, these symbols are the only thing that are processed, to prevent compiling dead code.</summary>
    public Dictionary<string, AmethystParser.FunctionDeclarationContext> EntryPointFunctions { get; } = new();

    /// <summary>The hierarchical tree structure representing the path segments of the source file.</summary>
    private readonly Node<string, SourceFile> _tree;

    /// <summary>Initializes a new instance of the <see cref="SourceFile" /> class using the provided path
    /// segments.</summary>
    /// <param name="pathSegments">The segments of the path representing the source file.</param>
    /// <param name="isInternal">Indicates whether the source file is internal, allowing reserved
    /// namespaces.</param>
    /// <exception cref="ArgumentException">Thrown when the path segments are invalid.</exception>
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

    /// <summary>Gets the full path of the source file, combining pack, namespace, registry, and the file
    /// path.</summary>
    /// <returns>The full path of the source file.</returns>
    public string GetFullPath()
    {
        return Path.Combine(Pack, Namespace, Registry, GetPath());
    }

    /// <summary>Gets the path of the source file relative to its pack, namespace, and registry.</summary>
    /// <returns>The relative path of the source file.</returns>
    public string GetPath()
    {
        return Path.Combine(_tree.Segments);
    }

    /// <summary>Gets the Minecraft function path of the source file in the format
    /// "namespace:path/to/function".</summary>
    public string McFunctionPath => $"{Namespace}:{GetPath().Replace(Path.DirectorySeparatorChar, '/')}";
    
    public override string ToString()
    {
        return GetPath() + SourceFileExtension;
    }

    /// <summary>A utility class to temporarily back up and restore the global state of the compiler
    /// (specifically the current source file and scope) using the IDisposable pattern.</summary>
    internal sealed class GlobalBackup : IDisposable
    {
        /// <summary>The compiler instance whose global state is being backed up and restored.</summary>
        private readonly Compiler _owner;

        /// <summary>The previous source file before the backup was created.</summary>
        private readonly SourceFile _previous;

        /// <summary>The previous scope before the backup was created.</summary>
        private readonly Scope _previousScope;

        /// <summary>Initializes a new instance of the <see cref="GlobalBackup" /> class, backing up</summary>
        /// <param name="owner">The compiler instance whose global state is being backed up.</param>
        /// <param name="sourceFile">The new source file to set in the compiler.</param>
        public GlobalBackup(Compiler owner, SourceFile sourceFile)
        {
            _owner = owner;
            _previous = owner.SourceFile;
            _previousScope = owner.Scope;
            owner.SourceFile = sourceFile;
            owner.Scope = sourceFile.Scope!;
        }
    
        /// <summary>Restores the previous global state of the compiler when the backup is disposed.</summary>
        public void Dispose()
        {
            _owner.SourceFile = _previous;
            _owner.Scope = _previousScope;
        }
    }
}