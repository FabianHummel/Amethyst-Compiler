using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitVariableDeclaration(AmethystParser.VariableDeclarationContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        if (TryGetSymbol(variableName, out _, context))
        {
            throw new SymbolAlreadyDeclaredException(variableName, context);
        }
        
        var result = VisitExpression(context.expression()).EnsureRuntimeValue();
        
        var type = GetOrInferTypeResult(result, context.type(), context);
        
        var name = result.Location;

        var attributes = VisitAttributeList(context.attributeList());

        Scope.Symbols.Add(variableName, new Variable
        {
            Name = variableName,
            Location = name,
            DataType = type,
            Attributes = attributes
        });
        
        return null;
    }
}