namespace Amethyst;

public static class Constants
{
    public const string CONFIG_FILE = "amethyst.toml";
    public const string SOURCE_FILE = "*.amy";
    public const string SOURCE_FILE_EXTENSION = ".amy"; 
    public const string SOURCE_DIRECTORY = "src";
    public const string MCFUNCTION_FILE_EXTENSION = ".mcfunction";
    public const string DEFAULT_DATAPACK_NAME = "Amethyst Datapack";
    public const string DEFAULT_RESOURCEPACK_NAME = "Amethyst Resourcepack";
    public const string DEFAULT_OUTPUT_DIRECTORY = "build";
    public const string DEFAULT_DATAPACK_DESCRIPTION = "An Amethyst Datapack";
    public const string DEFAULT_RESOURCEPACK_DESCRIPTION = "An Amethyst Resourcepack";
    public const int DEFAULT_DATAPACK_FORMAT = 48;
    public const int DEFAULT_RESOURCEPACK_FORMAT = 34;
    
    public static readonly string MINECRAFT_ROOT_WINDOWS = Environment.ExpandEnvironmentVariables("%APPDATA%\\.minecraft");
    public static readonly string MINECRAFT_ROOT_MACOS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library/Application Support/minecraft");
    public static readonly string MINECRAFT_ROOT_LINUX = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft");

    public const string ATTRIBUTE_LOAD_FUNCTION = "initializing";
    public const string ATTRIBUTE_TICK_FUNCTION = "ticking";
    public const string ATTRIBUTE_UNIT_TEST_FUNCTION = "test";
    public const string ATTRIBUTE_FUNCTION_NO_MANGLE = "no_mangle";
    public const string ATTRIBUTE_ASSIGN_ON_LEAVE = "assign_on_leave";
    public const string ATTRIBUTE_ASSIGN_ON_JOIN = "assign_on_join";
    public const string ATTRIBUTE_ASSIGN_ON_TICK = "assign_on_tick";
    public const string ATTRIBUTE_ASSIGN_ON_LOAD = "assign_on_load";
    
    public static readonly string AMETHYST_VERSION = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString();

    public static readonly IReadOnlyDictionary<string, string> SUBSTITUTIONS = new Dictionary<string, string>
    {
        { "pack_id", "{{pack_id}}" },
        { "description", "{{description}}" },
        { "pack_format", "{{pack_format}}" },
        { "loading_functions", "{{loading_functions}}" },
        { "ticking_functions", "{{ticking_functions}}" },
        { "date", "{{date}}" },
        { "amethyst_version", "{{amethyst_version}}" },
    };
    
    public static readonly IReadOnlyDictionary<string, object> SUBSTITUTION_VALUES = new Dictionary<string, object>
    {
        { "date", DateTime.Now.ToString("yyyy-MM-dd") },
        { "amethyst_version", AMETHYST_VERSION }
    };

    public const string DATAPACK_FUNCTIONS_DIRECTORY = "function";
    
    public static readonly IReadOnlySet<string> RESERVED_NAMESPACES = new HashSet<string>
    {
        "minecraft",
        "amethyst",
    };
}