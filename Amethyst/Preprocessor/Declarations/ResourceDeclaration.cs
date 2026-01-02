using System.Text.Json;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorResourceDeclaration(AmethystParser.PreprocessorResourceDeclarationContext context)
    {
        Scope.GlobalBackup? scopeBackup = null;
        if (context.preprocessorStringLiteral() is { } stringLiteralContext)
        {
            var relativePath = (PreprocessorString)Visit(stringLiteralContext)!;
            scopeBackup = this.EvaluateScoped(relativePath.Value, preserveName: true);
        }
        
        Scope.Type = ScopeType.Resource;
        
        var data = VisitPreprocessorObjectLiteral(context.preprocessorObjectLiteral());
        var json = JsonSerializer.Serialize(data);
        Scope.AddCode(json);

        scopeBackup?.Dispose();

        return null;
    }
}