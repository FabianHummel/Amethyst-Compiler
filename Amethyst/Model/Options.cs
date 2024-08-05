using CommandLine;

namespace Amethyst.Model;

// ReSharper disable once ClassNeverInstantiated.Global
public class Options
{
    [Option('w', "watch", Required = false, HelpText = "Recompile source files when they change")]
    public bool Watch { get; set; }
    
    [Option('d', "debug", Required = false, HelpText = "Enable debug output")]
    public bool Debug { get; set; }
}