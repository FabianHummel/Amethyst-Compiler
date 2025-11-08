using System.Diagnostics.CodeAnalysis;
using static Amethyst.Model.Constants;

namespace Amethyst.Model;

/// <summary>Represents a scope in the compilation process, managing symbols and code generation. There
/// is always one active scope during compilation at any given time, stored in
/// <see cref="Compiler.Scope" />.</summary>
public class Scope : IDisposable
{
    /// <summary>The name of this scope. This maps to the file or directory name of the generated
    /// <c>.mcfunction</c> file.</summary>
    public string Name { get; }

    /// <summary>The parent scope of this scope. Null if this is the root scope.</summary>
    /// <seealso cref="SourceFile.Scope" />
    public required Scope? Parent { get; init; }

    /// <summary>A dictionary mapping scope names to their occurrence counts, used to ensure unique naming
    /// of child scopes. This behaviour is overriden when using the <see cref="CompilerFlags.Debug" />
    /// flag, as scopes will retain their original names.</summary>
    public Dictionary<string, int> Scopes { get; } = new();

    /// <summary>A dictionary mapping symbol identifiers to their corresponding <see cref="Symbol" />
    /// objects.</summary>
    public Dictionary<string, Symbol> Symbols { get; } = new();

    /// <summary>The context in which this scope exists, providing access to configuration and other
    /// relevant data.</summary>
    private readonly Context _context;

    /// <summary>The source file associated with this scope, used for determining file paths and other
    /// related information.</summary>
    private readonly SourceFile _sourceFile;

    /// <summary>Indicates whether this scope has been cancelled. If true, no code will be written to the
    /// associated file upon disposal. This is useful for detecting later in the compilation process that a
    /// scope's code is not needed and can be skipped.</summary>
    /// <seealso cref="Compiler.VisitConjunctionExpression" />
    /// <seealso cref="Compiler.VisitDisjunctionExpression" />
    private bool _isCancelled;

    /// <summary>A writer that accumulates the code generated within this scope. This code will be written
    /// to the appropriate <c>.mcfunction</c> file upon disposal of the scope.</summary>
    private TextWriter? _writer;

    /// <summary>Initializes a new instance of the <see cref="Scope" /> class with an empty name.</summary>
    /// <param name="context">The compilation context.</param>
    /// <param name="sourceFile">The source file associated with this scope.</param>
    public Scope(Context context, SourceFile sourceFile)
        : this("", context, sourceFile)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="Scope" /> class with the specified name,
    /// context, and source file.</summary>
    /// <param name="name">The name of the scope.</param>
    /// <param name="context">The compilation context.</param>
    /// <param name="sourceFile">The source file associated with this scope.</param>
    public Scope(string name, Context context, SourceFile sourceFile)
    {
        Name = name;
        _context = context;
        _sourceFile = sourceFile;
    }

    /// <summary>The absolute file path where the directory or generated <c>.mcfunction</c> file for this
    /// scope will be saved.</summary>
    public string FilePath => Path.Combine(
        _context.Configuration.Datapack!.OutputDir, 
        _sourceFile.GetFullPath(),
        GetPath(callerIsRoot: IsRoot) + McfunctionFileExtension);


    /// <summary>Gets the relative path of this scope by combining the names of its parent scopes. Used to
    /// assemble the absolute file path in <see cref="FilePath" />.</summary>
    /// <returns></returns>
    public string GetPath()
    {
        return Path.Combine(Parent?.GetPath() ?? "", Name);
    }

    /// <summary>The Minecraft function path for this scope, formatted with forward slashes and prefixed by
    /// the source file's function path, resulting in a complete function path usable within Minecraft.</summary>
    
    public string McFunctionPath
    {
        get
        {
            var path = GetPath(callerIsRoot: IsRoot);
            var mcFunctionPath = path.Replace(Path.DirectorySeparatorChar, '/');
            return $"{_sourceFile.McFunctionPath}/{mcFunctionPath}";
        }
    }
    
    public bool IsRoot => Parent == null;

    /// <summary>Adds a line of code to the scope's internal writer, which will be written to the
    /// associated <c>.mcfunction</c> file upon disposal.</summary>
    /// <param name="code">The MCFunction code to add.</param>
    public void AddCode(string code)
    {
        if (_writer == null)
        {
            _writer = new StringWriter();
        }
        
        _writer.WriteLine(code);
    }

    /// <summary>Marks this scope as cancelled, preventing any code from being written to the associated
    /// file upon disposal. This is useful for skipping unnecessary scopes during the compilation process.</summary>
    public void Cancel()
    {
        _isCancelled = true;
    }

    /// <summary>Attempts to retrieve a symbol by its identifier from the current scope or any parent
    /// scopes.</summary>
    /// <param name="identifier">The identifier of the symbol to retrieve.</param>
    /// <param name="symbol">When this method returns, contains the symbol associated with the specified
    /// identifier, if found; otherwise, null.</param>
    /// <returns>>true if the symbol was found; otherwise, false.</returns>
    public bool TryGetSymbol(string identifier, [NotNullWhen(true)] out Symbol? symbol)
    {
        if (Symbols.TryGetValue(identifier, out symbol))
        {
            return true;
        }
        
        if (Parent is not null)
        {
            return Parent.TryGetSymbol(identifier, out symbol);
        }
        
        return false;
    }

    /// <summary>Disposes of the scope, writing any accumulated code to the associated <c>.mcfunction</c>
    /// file unless the scope has been cancelled.</summary>
    public void Dispose()
    {
        if (_isCancelled || _writer == null)
        {
            GC.SuppressFinalize(this);
            return;
        }

        if (IsRoot)
        {
            _context.Configuration.Datapack!.LoadFunctions.Add(McFunctionPath);
        }
        
        if (!File.Exists(FilePath))
        {
            Processor.CreateFunctionFile(FilePath);
        }
        File.AppendAllText(FilePath, _writer.ToString());
        _writer.Dispose();
        
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return $"{McFunctionPath} ({FilePath})";
    }

    /// <summary>Creates a new child scope under the specified parent scope with a unique name.</summary>
    /// <param name="parent">The parent scope under which the new scope will be created.</param>
    /// <param name="name">The base name for the new scope.</param>
    /// <param name="preserveName">If true, the new scope will retain the provided name without
    /// modification.</param>
    /// <returns>The newly created child scope.</returns>
    public static Scope Reparent(Scope parent, string name, bool preserveName = false)
    {
        if (preserveName)
        {
            goto createScope;
        }
        
        if (!parent.Scopes.TryAdd(name, 0))
        {
            parent.Scopes[name]++;
        }
        
        name += parent.Scopes[name];
        
    createScope:
        return new Scope(name, parent._context, parent._sourceFile)
        {
            Parent = parent
        };
    }
    
    private string GetPath(bool callerIsRoot)
    {
        var path = callerIsRoot && IsRoot ? InitFunctionName : Parent?.GetPath(callerIsRoot);
        return Path.Combine(path ?? "", Name);
    }
    

    /// <summary>A helper class that temporarily sets the global scope of a compiler to a new scope and
    /// restores the previous scope upon disposal.</summary>
    internal sealed class GlobalBackup : IDisposable
    {
        /// <summary>The compiler whose global scope is being managed.</summary>
        private readonly Compiler _owner;

        /// <summary>The previous scope of the compiler, to be restored upon disposal.</summary>
        private readonly Scope _previous;

        /// <summary>Initializes a new instance of the <see cref="GlobalBackup" /> class, setting the
        /// compiler's scope to the specified new scope.</summary>
        /// <param name="owner">The compiler whose scope is to be changed.</param>
        /// <param name="scope">The new scope to set for the compiler.</param>
        public GlobalBackup(Compiler owner, Scope scope)
        {
            _owner = owner;
            _previous = owner.Scope;
            owner.Scope = scope;
        }

        /// <summary>Restores the compiler's previous scope when this object is disposed.</summary>
        public void Dispose()
        {
            Dispose(disposeScope: true);
        }

        public void Dispose(bool disposeScope)
        {
            if (disposeScope)
            {
                _owner.Scope.Dispose();
            }
            _owner.Scope = _previous;
        }
    }
}

/// <summary>Extension methods for the <see cref="Compiler" /> class to facilitate scope management and
/// code generation.</summary>
public static partial class CompilerExtensions
{
    /// <inheritdoc cref="Scope.AddCode(string)" />
    public static void AddCode(this Compiler compiler, string code)
    {
        compiler.Scope.AddCode(code);
    }

    /// <summary>Creates a new scoped context for the compiler, allowing for code generation within the
    /// specified scope name. The previous scope is restored upon disposal of the returned
    /// <see cref="Scope.GlobalBackup" /> object.</summary>
    /// <param name="compiler">The compiler for which to create the scoped context.</param>
    /// <param name="name">The name of the new scope.</param>
    /// <param name="preserveName">If true, the new scope will retain the provided name without
    /// modification.</param>
    /// <returns>A <see cref="Scope.GlobalBackup" /> object that restores the previous scope upon disposal.</returns>
    internal static Scope.GlobalBackup EvaluateScoped(this Compiler compiler, string name, bool preserveName = false)
    {
        var newScope = Scope.Reparent(compiler.Scope, name, preserveName);
        return new Scope.GlobalBackup(compiler, newScope);
    }
}