using Amethyst.Model;
using CommandLine;
using static Amethyst.Constants;
using static Amethyst.Utility.ConsoleUtility;

namespace Amethyst;

/// <summary>The class that contains the main entrypoint of the compiler. More info at the
/// <see cref="Main">main</see> function.</summary>
public static class Program
{
    private static FileSystemWatcher _srcWatcher = null!;
    private static FileSystemWatcher _configWatcher = null!;
    private static Processor _amethyst = null!;
    private static CancellationTokenSource? _onChangedSourceTokenSource;

    /// <summary>The main entrypoint for the compiler. First parses command line arguments, prints the
    /// logo, sets up file watchers (<see cref="SetupFileWatchers" />) and compiles the program.</summary>
    /// <param name="args">Command line arguments passed to the compiler. These are the valid
    /// <see cref="Options">options</see>.</param>
    private static void Main(string[] args)
    {
        CommandLine.Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
        {
            CompilerFlags compilerFlags = 0;
            if (o.Debug) compilerFlags |= CompilerFlags.Debug;
            if (o.Watch) compilerFlags |= CompilerFlags.Watch;
            if (o.ReduceColors) IsReducedColors = true;

            var rootDir = o.Path != null
                ? Path.Combine(Environment.CurrentDirectory, o.Path)
                : Environment.CurrentDirectory;
            
            ClearConsole();
            Processor.PrintAmethystLogoAndVersion(IsReducedColors, compilerFlags);
            _amethyst = new Processor(rootDir, compilerFlags);
            
            if (o.Watch)
            {
                SetupFileWatchers(rootDir);
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

    /// <summary>Sets up file watchers to run actions when specific files change:
    /// <list type="number">
    ///     <item>When files in the source folder change, <see cref="OnChangedSource" /> is run.</item>
    ///     <item>When the amethyst configuration file changes, <see cref="OnChangedConfig" /> is run
    ///     instead.</item>
    /// </list>
    /// </summary>
    /// <param name="rootDir">The directory where to look for the amethyst configuration and source folder</param>
    private static void SetupFileWatchers(string rootDir)
    {
        var thread = new Thread(() =>
        {
            _srcWatcher = new FileSystemWatcher();
            _srcWatcher.Path = Path.Combine(rootDir, SourceDirectory);
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
            _configWatcher.Path = rootDir;
            _configWatcher.NotifyFilter = NotifyFilters.Attributes |
                                         NotifyFilters.CreationTime |
                                         NotifyFilters.FileName |
                                         NotifyFilters.LastAccess |
                                         NotifyFilters.LastWrite |
                                         NotifyFilters.Size |
                                         NotifyFilters.Security;
            _configWatcher.Filters.Add(ConfigFile);
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

    /// <summary>Run when anything in the source directory changes. Recompiles the project.</summary>
    private static void OnChangedSource(object sender, FileSystemEventArgs e)
    {
        _onChangedSourceTokenSource?.Cancel();
        _onChangedSourceTokenSource = new CancellationTokenSource();
        Task.Delay(100, _onChangedSourceTokenSource.Token).ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
            {
                ClearConsole();
                Processor.PrintAmethystLogoAndVersion(IsReducedColors, _amethyst.Context.CompilerFlags);
                _amethyst = new Processor(_amethyst.Context);
            }
        }, TaskScheduler.Default);
    }

    /// <summary>Run when the amethyst configuration is changed. Reloads the entire config and recompile
    /// the project.</summary>
    private static void OnChangedConfig(object sender, FileSystemEventArgs e)
    {
        _onChangedSourceTokenSource?.Cancel();
        _onChangedSourceTokenSource = new CancellationTokenSource();
        Task.Delay(100, _onChangedSourceTokenSource.Token).ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
            {
                ClearConsole();
                Processor.PrintAmethystLogoAndVersion(IsReducedColors, _amethyst.Context.CompilerFlags);
                Console.WriteLine("Configuration changed, reinitializing project...");
                _amethyst = new Processor(_amethyst.Context.RootDir, _amethyst.Context.CompilerFlags);
            }
        }, TaskScheduler.Default);
    }
}