using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override Symbol VisitVariableDeclaration(AmethystParser.VariableDeclarationContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        if (EnsureSymbolIsNewOrGetRootSymbol(variableName, context, out var symbol))
        {
            return symbol;
        }
        
        var result = VisitExpression(context.expression()).EnsureRuntimeValue();
        
        var type = GetOrInferTypeResult(result, context.type(), context);
        
        var name = result.Location;

        var attributes = VisitAttributeList(context.attributeList());

        var variable = new Variable
        {
            Name = variableName,
            Location = name,
            Datatype = type,
            Attributes = attributes
        };
        
        Scope.Symbols.Add(variableName, variable);
        
        return variable;
    }
}