using static Amethyst.Model.Constants;

namespace Amethyst.Model;

/// <summary>A resource that is used to identify resource files such as models, textures or other source code files.
/// It applies the same auto-completion rules to resource paths as Minecraft.</summary>
/// <example><p><c>`my_namespace:path/to/resource`</c> → <c>`my_namespace:path/to/resource`</c></p>
///     <p><c>`:path/to/resource`</c> → <c>`minecraft:path/to/resource`</c></p>
///     <p><c>`my_namespace:`</c> → <c>`my_namespace:`</c></p>
///     <p><c>`:`</c> → <c>`minecraft:`</c></p></example>
public class Resource
{
    public string Namespace { get; }
    public string Path { get; }
    
    public Resource(string resourceLiteral)
    {
        resourceLiteral = resourceLiteral.Trim();
        
        if (!resourceLiteral.Contains(':'))
        {
            resourceLiteral = $":{resourceLiteral}";
        }
        
        var parts = resourceLiteral.Split(":", 2);
        
        Namespace = parts[0];
        if (Namespace.IsWhiteSpace())
        {
            Namespace = MinecraftNamespaceName;
        }
        
        Path = parts[1];
    }
    
    public override string ToString()
    {
        return $"{Namespace}:{Path}";
    }

    public static implicit operator string(Resource resource)
    {
        return resource.ToString();
    }
}