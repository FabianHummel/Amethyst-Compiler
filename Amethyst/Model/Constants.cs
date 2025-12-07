using System.Globalization;

namespace Amethyst.Model;

/// <summary>Collection of constants that are used throughout the compiler.</summary>
public static partial class Constants
{
    public const string ConfigFile = "amethyst.toml";
    public const string SourceFile = "*.amy";
    public const string SourceFileExtension = ".amy"; 
    public const string SourceDirectory = "src";
    public const string McfunctionFileExtension = ".mcfunction";
    public const string DefaultDatapackName = "Amethyst Datapack";
    public const string DefaultResourcepackName = "Amethyst Resourcepack";
    public const string DefaultOutputDirectory = "build";
    public const string DefaultDatapackDescription = "An Amethyst Datapack";
    public const string DefaultResourcepackDescription = "An Amethyst Resourcepack";
    public const int DefaultDatapackFormat = 48;
    public const int DefaultResourcepackFormat = 34;
    
    public static readonly string MinecraftRootWindows = Environment.ExpandEnvironmentVariables("%APPDATA%\\.minecraft");
    public static readonly string MinecraftRootMacos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/minecraft");
    public static readonly string MinecraftRootLinux = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft");

    public const string AttributeLoadFunction = "initializing";
    public const string AttributeTickFunction = "ticking";
    public const string AttributeUnitTestFunction = "test";
    public const string AttributeNoMangle = "no_mangle";
    // public const string AttributeAssignOnLeave = "assign_on_leave";
    // public const string AttributeAssignOnJoin = "assign_on_join";
    // public const string AttributeAssignOnTick = "assign_on_tick";
    // public const string AttributeAssignOnLoad = "assign_on_load";
    
    public const string DatapackRootDir = "data";
    public const string ResourceRootDir = "assets";
    public const string InternalNamespaceName = "amethyst";
    public const string DatapackFunctionsDirectory = "function";
    public const string InitFunctionName = "_init";
    public const string MinecraftNamespaceName = "minecraft";

    public static readonly string AmethystVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

    /// <summary>Collection of mustache bindings that are used to substitute specific parts in template
    /// files.</summary>
    public static readonly IReadOnlyDictionary<string, string> Substitutions = new Dictionary<string, string>
    {
        { "pack_id", "{{pack_id}}" },
        { "description", "{{description}}" },
        { "pack_format", "{{pack_format}}" },
        { "loading_functions", "{{loading_functions}}" },
        { "ticking_functions", "{{ticking_functions}}" },
        { "date", "{{date}}" },
        { "amethyst_version", "{{amethyst_version}}" }
    };

    /// <summary>Predefined values that are used to substitute values of <see cref="Substitutions" />.</summary>
    public static readonly IReadOnlyDictionary<string, object> SubstitutionValues = new Dictionary<string, object>
    {
        { "date", DateTime.Now.ToString("yyyy-MM-dd", NumberFormatInfo.InvariantInfo) },
        { "amethyst_version", AmethystVersion }
    };
}