using CommandLine;
using JetBrains.Annotations;

namespace Amethyst.Model;

/// <summary>Command line options for the Amethyst compiler. Some of these options are parsed directly
/// into <see cref="CompilerFlags" />.</summary>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class Options
{
    /// <inheritdoc cref="CompilerFlags.Watch" />
    [Option('w', "watch", Required = false, HelpText = "Recompile source files when they change")]
    public bool Watch { get; [UsedImplicitly] init; }
    
    /// <inheritdoc cref="CompilerFlags.Debug" />
    [Option('d', "debug", Required = false, HelpText = "Enable debug output")]
    public bool Debug { get; [UsedImplicitly] init; }

    /// <summary>Fall back to print plain text in the console. This is due to issues with some terminal
    /// emulators and console outputs.</summary>
    [Option("reduce-colors", Required = false, HelpText = "Reduce colors in console output")]
    public bool ReduceColors { get; [UsedImplicitly] init; }

    /// <summary>Path to the project's root directory. This option can be used if the current working
    /// directory is not the project root.</summary>
    [Option('p', "path", Required = false, HelpText = "Path to the project directory")]
    public string? Path { get; [UsedImplicitly] init; }
}