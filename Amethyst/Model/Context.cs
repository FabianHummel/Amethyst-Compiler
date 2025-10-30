namespace Amethyst.Model;

/// <summary>The compilation context containing the project's configuration, a list of source files and
/// other relevant data for the compilation process. It can be created from scratch or copied from an
/// existing context..</summary>
public class Context
{
    /// <summary>The path where to look for the project's configuration and source directory.</summary>
    /// <example>
    ///     <code>
    /// /path/to/project    (rootDir)
    ///  ├── amethyst.toml
    ///  └── src
    ///  </code>
    /// </example>
    public string RootDir { get; }

    /// <inheritdoc cref="Model.CompilerFlags" />
    public CompilerFlags CompilerFlags { get; }

    /// <summary>The path to the source directory of the project. Usually a combination of
    /// <see cref="RootDir" /> and <see cref="Constants.SourceDirectory" />.</summary>
    public string SourcePath { get; }

    /// <inheritdoc cref="Model.Configuration" />
    public Configuration Configuration { get; }

    /// <summary>A list of source files that are part of the project, indexed by their full path. In the
    /// compiler, these are individually processed and compiled (<see cref="Compiler.CompileSourceFile" />
    /// ).</summary>
    public Dictionary<string, SourceFile> SourceFiles { get; } = new();

    /// <summary>A list of entry points for unit tests, indexed by their MCFunction path, so they can be
    /// resolved by the test runner.</summary>
    public Dictionary<string, Scope> UnitTests { get; } = new();

    /// <summary>Creates a new compilation context from scratch. This option is used upon the first
    /// initialization or when the project's configuration changes and the context needs to be rebuilt.</summary>
    /// <param name="rootDir">The path where to look for the project's configuration and source directory.</param>
    /// <param name="compilerFlags">The compiler flags to use for this compilation.</param>
    /// <param name="configuration">The project's configuration.</param>
    public Context(string rootDir, CompilerFlags compilerFlags, Configuration configuration)
    {
        RootDir = rootDir;
        CompilerFlags = compilerFlags;
        Configuration = configuration;
        SourcePath = Path.Combine(RootDir, Constants.SourceDirectory);
    }

    /// <summary>Creates a new compilation context by copying an existing one. This option is used when the
    /// project's source files change but the configuration remains the same.</summary>
    /// <param name="copy">The existing context to copy.</param>
    public Context(Context copy)
    {
        RootDir = copy.RootDir;
        CompilerFlags = copy.CompilerFlags;
        SourcePath = copy.SourcePath;
        Configuration = copy.Configuration;
    }
}