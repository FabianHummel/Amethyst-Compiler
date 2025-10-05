namespace Amethyst.Model;

public class Context
{
    public string RootDir { get; }
    public CompilerFlags CompilerFlags { get; }
    public string SourcePath { get; }
    public Configuration Configuration { get; }
    public Dictionary<string, Namespace> Namespaces { get; } = new();
    public Dictionary<string, Scope> UnitTests { get; } = new();

    public Context(string rootDir, CompilerFlags compilerFlags, Configuration configuration)
    {
        RootDir = rootDir;
        CompilerFlags = compilerFlags;
        Configuration = configuration;
        SourcePath = Path.Combine(RootDir, Constants.SOURCE_DIRECTORY);
    }

    public Context(Context copy)
    {
        RootDir = copy.RootDir;
        CompilerFlags = copy.CompilerFlags;
        SourcePath = copy.SourcePath;
        Configuration = copy.Configuration;
    }
}