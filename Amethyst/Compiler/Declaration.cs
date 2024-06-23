using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitDeclaration(AmethystParser.DeclarationContext context)
    {
        if (context.namespace_declaration() is { } namespaceDeclaration)
        {
            return null;
        }
        if (context.function_declaration() is { } functionDeclaration)
        {
            return null;
        }
        if (context.statement() is { } statement)
        {
            return null;
        }
    
        throw new UnreachableException();
    }
}