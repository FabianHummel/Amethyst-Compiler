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
        if (context.variable_declaration() is { } variableDeclaration)
        {
            return VisitVariable_declaration(variableDeclaration);
        }
        if (context.record_declaration() is { } recordDeclaration)
        {
            return VisitRecord_declaration(recordDeclaration);
        }
    
        throw new UnreachableException();
    }
}