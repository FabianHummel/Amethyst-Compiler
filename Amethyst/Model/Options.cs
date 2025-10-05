using CommandLine;
using JetBrains.Annotations;

namespace Amethyst.Model;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class Options
{
    [Option('w', "watch", Required = false, HelpText = "Recompile source files when they change")]
    public bool Watch { get; [UsedImplicitly] init; }
    
    [Option('d', "debug", Required = false, HelpText = "Enable debug output")]
    public bool Debug { get; [UsedImplicitly] init; }
    
    [Option("reduce-colors", Required = false, HelpText = "Reduce colors in console output")]
    public bool ReduceColors { get; [UsedImplicitly] init; }
    
    [Option('p', "path", Required = false, HelpText = "Path to the project directory")]
    public string? Path { get; [UsedImplicitly] init; }
}