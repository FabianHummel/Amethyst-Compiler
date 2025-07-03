using Amethyst.Model;
using Amethyst.Utility;
using CommandLine;
using static Amethyst.Constants;
using static Amethyst.Utility.ConsoleUtility;

namespace Amethyst;

public static class Program
{
    private static FileSystemWatcher _srcWatcher = null!;
    private static FileSystemWatcher _configWatcher = null!;
    private static CancellationTokenSource? _onChangedSourceTokenSource;
    private static readonly Processor _amethyst = new();
    
    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if (o.Debug)
                {
                    _amethyst.Context.CompilerFlags |= CompilerFlags.Debug;
                }
    
                if (o.Watch)
                {
                    _amethyst.Context.CompilerFlags |= CompilerFlags.Watch;
                    ClearConsole();
                }
                
                _amethyst.PrintAmethystLogoAndVersion();
                _amethyst.ReinitializeProject();
                
                if (o.Watch)
                {
                    var thread = new Thread(() =>
                    {
                        _srcWatcher = new FileSystemWatcher();
                        _srcWatcher.Path = Path.Combine(Environment.CurrentDirectory, SOURCE_DIRECTORY);
                        _srcWatcher.NotifyFilter = NotifyFilters.Attributes |
                                                NotifyFilters.CreationTime |
                                                NotifyFilters.FileName |
                                                NotifyFilters.LastAccess |
                                                NotifyFilters.LastWrite |
                                                NotifyFilters.Size |
                                                NotifyFilters.Security;
                        _srcWatcher.Filters.Add("*.*");
                        _srcWatcher.Changed += OnChangedSource;
                        _srcWatcher.Created += OnChangedSource;
                        _srcWatcher.Deleted += OnChangedSource;
                        _srcWatcher.Renamed += OnChangedSource;
                        _srcWatcher.EnableRaisingEvents = true;
                        _srcWatcher.IncludeSubdirectories = true;
                        
                        _configWatcher = new FileSystemWatcher();
                        _configWatcher.Path = Environment.CurrentDirectory;
                        _configWatcher.NotifyFilter = NotifyFilters.Attributes |
                                                     NotifyFilters.CreationTime |
                                                     NotifyFilters.FileName |
                                                     NotifyFilters.LastAccess |
                                                     NotifyFilters.LastWrite |
                                                     NotifyFilters.Size |
                                                     NotifyFilters.Security;
                        _configWatcher.Filters.Add(CONFIG_FILE);
                        _configWatcher.Changed += OnChangedConfig;
                        _configWatcher.Created += OnChangedConfig;
                        _configWatcher.Deleted += OnChangedConfig;
                        _configWatcher.Renamed += OnChangedConfig;
                        _configWatcher.EnableRaisingEvents = true;
                    });
                    thread.Start();
    
                    while (Console.ReadLine() != "exit")
                    {
                        
                    }
                }
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    PrintError($"Invalid argument {error.Tag}");
                }
            });
    }
    
    private static void OnChangedSource(object sender, FileSystemEventArgs e)
    {
        _onChangedSourceTokenSource?.Cancel();
        _onChangedSourceTokenSource = new CancellationTokenSource();
        Task.Delay(100, _onChangedSourceTokenSource.Token).ContinueWith(t => {
            if (t.IsCompletedSuccessfully)
            {
                ClearConsole();
                _amethyst.PrintAmethystLogoAndVersion();
                _amethyst.RecompileProject();
            }
        }, TaskScheduler.Default);
    }
    
    private static void OnChangedConfig(object sender, FileSystemEventArgs e)
    {
        _onChangedSourceTokenSource?.Cancel();
        _onChangedSourceTokenSource = new CancellationTokenSource();
        Task.Delay(100, _onChangedSourceTokenSource.Token).ContinueWith(t => {
            if (t.IsCompletedSuccessfully)
            {
                ClearConsole();
                _amethyst.PrintAmethystLogoAndVersion();
                Console.WriteLine("Configuration changed, reinitializing project...");
                _amethyst.ReinitializeProject();
            }
        }, TaskScheduler.Default);
    }
}