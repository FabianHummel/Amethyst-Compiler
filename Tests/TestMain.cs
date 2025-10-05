using System.Diagnostics;
using System.Text;
using Amethyst;
using Microsoft.Extensions.Configuration;
using RconSharp;

namespace Tests;

public static partial class TestMain
{
    private static readonly MinecraftServerSettings _settings;
    private static Process _process = null!;
    
    public static RconClient Rcon { get; private set; } = null!;
    public static Processor Amethyst { get; private set; } = null!;

    static TestMain()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        _settings = configuration.GetSection("MinecraftServer").Get<MinecraftServerSettings>() 
                    ?? throw new InvalidOperationException("Failed to load MinecraftServer settings.");
        
        var serverPropertiesTemplate = Path.Combine(Environment.CurrentDirectory, "server.properties.template");
        if (!File.Exists(serverPropertiesTemplate))
        {
            throw new FileNotFoundException("server.properties.template not found.");
        }
        
        var serverProperties = Path.Combine(Environment.CurrentDirectory, "server.properties");
        File.WriteAllText(serverProperties, File.ReadAllText(serverPropertiesTemplate, Encoding.UTF8)
            .Replace("{RCON_PASSWORD}", _settings.RconPassword)
            .Replace("{RCON_PORT}", _settings.RconPort.ToString()));
    }
}