using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public SourceFile VisitResource(string resourcePath, string registryName)
    {
        var ns = Namespace;

        if (resourcePath.Contains(':'))
        {
            var parts = resourcePath.Split(':', 2);
            var nsName = parts[0];
            if (!Context.Namespaces.TryGetValue(nsName, out ns))
            {
                throw new SemanticException($"Namespace '{nsName}' does not exist in the project.");
            }
            resourcePath = parts[1];
            if (resourcePath == null)
            {
                throw new SemanticException("Resource path cannot be empty.");
            }
        }
        
        var paths = resourcePath.Split('/');
        var name = paths[^1];
        paths = paths[..^1];

        if (!ns.Registries.TryGetValue(registryName, out var sourceFolder))
        {
            throw new SemanticException($"No symbols have been defined in the '{registryName}' registry.");
        }
        
        foreach (var sourceFolderName in paths)
        {
            if (!sourceFolder.Children.TryGetValue(sourceFolderName, out sourceFolder))
            {
                throw new SemanticException($"The folder '{sourceFolderName}' does not exist for the specified path.");
            }
        }
        
        if (!sourceFolder.SourceFiles.TryGetValue(name, out var sourceFile))
        {
            throw new SemanticException($"The file '{name}' does not exist for the specified path.");
        }

        return sourceFile;
    }
}