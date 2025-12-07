namespace Amethyst.Model;

/// <summary>A list of compiler flags that can be set when compiling a project. They are usually passed
/// to the compiler as command line arguments at <see cref="Program.Main" />.</summary>
[Flags]
public enum CompilerFlags
{
    /// <summary>Watch for file changes and recompile the project automatically.
    /// <seealso cref="Program.SetupFileWatchers" /></summary>
    Watch = 1,

    /// <summary>Whether to include debug information in the compiled output. This includes debug names for
    /// compiled symbols and extra information printed to the console during compilation.</summary>
    Debug = 2
}