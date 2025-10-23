using System.Diagnostics;
using System.Globalization;
using System.Reflection;
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
        
        var resourceName = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(
            n => n.EndsWith("server.properties.template", StringComparison.OrdinalIgnoreCase));
        if (resourceName == null)
        {
            throw new FileNotFoundException("Embedded resource `server.properties.template` not found.");
        }

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var serverProperties = Path.Combine(Environment.CurrentDirectory, "server.properties");
        File.WriteAllText(serverProperties, reader.ReadToEnd()
            .Replace("{RCON_PASSWORD}", _settings.RconPassword)
            .Replace("{RCON_PORT}", _settings.RconPort.ToString(NumberFormatInfo.InvariantInfo)));
    }
}