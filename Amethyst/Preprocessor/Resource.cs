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
        
        var pathSegments = resourcePath.Split('/');
        var name = pathSegments[^1];

        if (!ns.Registries.TryGetValue(registryName, out var registry))
        {
            throw new SemanticException($"No symbols have been defined in the '{registryName}' registry.", context);
        }
        
        if (!registry.Root.TryTraverse(pathSegments[..^1], out var sourceFolder))
        {
            throw new SemanticException($"Could not find a part of the path '{resourcePath}'.", context);
        }
        
        if (!sourceFolder.Leaves.TryGetValue(name, out var sourceFile))
        {
            throw new SemanticException($"The file '{name}' does not exist for the specified path.", context);
        }

        return sourceFile.Value;
    }
}