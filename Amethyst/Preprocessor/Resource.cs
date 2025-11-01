using Amethyst.Model;
using Antlr4.Runtime;
using static Amethyst.Model.Constants;

namespace Amethyst;

public partial class Compiler
{
    public SourceFile VisitResource(string resourcePath, string registryName, ParserRuleContext context)
    {
        var nsName = SourceFile.Namespace;

        if (resourcePath.Contains(':'))
        {
            var parts = resourcePath.Split(':', 2);
            nsName = parts[0];
            resourcePath = parts[1];
            if (resourcePath == null)
            {
                throw new SyntaxException("Resource path cannot be empty.", context);
            }
        }

        var sourceFilePath = Path.Combine(DatapackRootDir, nsName, registryName, resourcePath);
        
        if (!Context.SourceFiles.TryGetValue(sourceFilePath, out var sourceFile))
        {
            throw new SemanticException($"The source file '{sourceFilePath}' does not exist for the specified path.", context);
        }

        return sourceFile;
    }
}