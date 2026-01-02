using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorImageDeclaration(AmethystParser.PreprocessorImageDeclarationContext context)
    {
        Scope.GlobalBackup? scopeBackup = null;
        if (context.preprocessorStringLiteral() is { } stringLiteralContext)
        {
            var relativePath = (PreprocessorString)Visit(stringLiteralContext)!;
            scopeBackup = this.EvaluateScoped(relativePath.Value, preserveName: true);
        }
        
        Scope.Type = ScopeType.Image;
        
        var data = VisitPreprocessorArrayLiteral(context.preprocessorArrayLiteral());
        if (data.Any(value => value is not PreprocessorColor))
        {
            throw new SyntaxException("Image data must be an array of colors.", context);
        }

        var imageBounds = context.IMAGE_BOUNDS().GetText().Split('x');
        var width = int.Parse(imageBounds[0]);
        var height = int.Parse(imageBounds[1]);
        var png = new SimplePng(Scope._memoryStream, width, height);

        int x = 0, y = 0;
        foreach (var preprocessorValue in data)
        {
            var color = (PreprocessorColor)preprocessorValue;
            png.Write(x, y, color.Value);
            
            x = (x + 1) % width;
            if (x == 0) y++;
        }
        
        png.Save();

        scopeBackup?.Dispose();

        return null;
    }
}