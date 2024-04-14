using CommandLine;

namespace Amethyst;

// ReSharper disable once ClassNeverInstantiated.Global
public class Options
{
    [Option('w', "watch", Required = false, HelpText = "Recompile source files when they change")]
    public bool Watch { get; set; }
}