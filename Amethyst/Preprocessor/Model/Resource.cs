using static Amethyst.Model.Constants;

namespace Amethyst.Model;

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