using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitVariable_declaration(AmethystParser.Variable_declarationContext context)
    {
        var variableName = context.identifier().GetText();
        
        if (Scope.Variables.ContainsKey(variableName))
        {
            throw new SyntaxException($"The variable '{variableName}' has already been declared.", context);
        }
        
        if (context.expression() is not { } expressionContext)
        {
            throw new SyntaxException("Expected expression.", context);
        }
        
        var result = VisitExpression(expressionContext).ToRuntimeValue();

        var name = result.Location;
        
        DataType? type = null;
        
        // if a type is defined, set the type to the defined type
        if (context.type() is { } typeContext)
        {
            type = VisitType(typeContext);
        }
        // if both types are defined, check if they match
        if (type != null && result != null && type != result.DataType)
        {
            throw new SyntaxException($"The type of the variable '{type}' does not match the inferred type '{result.DataType}'.", context);
        }
        // if no type is defined or inferred, throw an error
        if (type == null && result == null)
        {
            throw new SyntaxException("Variable declarations must have specified a type or an expression to infer the type from.", context);
        }
        // if no type is defined, but inferred, set the type to the inferred type
        if (type == null && result != null)
        {
            type = result.DataType;
        }
        
        Debug.Assert(type != null, nameof(type) + " != null");

        var attributes = VisitAttribute_list(context.attribute_list());

        Scope.Variables.Add(variableName, new Variable
        {
            Location = name,
            DataType = type,
            Attributes = attributes
        });
        
        return null;
    }
}