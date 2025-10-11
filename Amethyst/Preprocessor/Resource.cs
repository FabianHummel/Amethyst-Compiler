using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public SourceFile VisitResource(string resourcePath, string registryName, ParserRuleContext context, bool symbolIsLastPathSection = true)
    {
        var ns = Namespace;

        if (resourcePath.Contains(':'))
        {
            var parts = resourcePath.Split(':', 2);
            var nsName = parts[0];
            if (!Context.Namespaces.TryGetValue(nsName, out ns))
            {
                throw new SemanticException($"Namespace '{nsName}' does not exist in the project.", context);
            }
            resourcePath = parts[1];
            if (resourcePath == null)
            {
                throw new SyntaxException("Resource path cannot be empty.", context);
            }
        }
        
        var paths = resourcePath.Split('/');
        var name = paths[^1];
        paths = paths[..^1];

        if (!ns.Registries.TryGetValue(registryName, out var sourceFolder))
        {
            throw new SemanticException($"No symbols have been defined in the '{registryName}' registry.", context);
        }
        
        foreach (var sourceFolderName in paths)
        {
            if (!sourceFolder.Children.TryGetValue(sourceFolderName, out sourceFolder))
            {
                throw new SemanticException($"The folder '{sourceFolderName}' does not exist for the specified path.", context);
            }
        }
        
        if (!sourceFolder.SourceFiles.TryGetValue(name, out var sourceFile))
        {
            throw new SemanticException($"The file '{name}' does not exist for the specified path.", context);
        }

        return sourceFile;
    }
}