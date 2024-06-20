using CommandLine;

namespace Amethyst.Model;

// ReSharper disable once ClassNeverInstantiated.Global
public class Options
{
    [Option('w', "watch", Required = false, HelpText = "Recompile source files when they change")]
    public bool Watch { get; set; }
}