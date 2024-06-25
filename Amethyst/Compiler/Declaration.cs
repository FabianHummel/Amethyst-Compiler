using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitDeclaration(AmethystParser.DeclarationContext context)
    {
        if (context.function_declaration() is { } functionDeclaration)
        {
            return VisitFunction_declaration(functionDeclaration);
        }
        if (context.statement() is { } statement)
        {
            return VisitStatement(statement);
        }
    
        throw new UnreachableException();
    }
}