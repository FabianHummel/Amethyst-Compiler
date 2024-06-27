using Amethyst.Language;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    private int _variableIndex = 0;
    private int _recordIndex = 0;
    
    public string GetVariableName()
    {
        return $"$v{_variableIndex++}";
    }
    
    public string GetRecordName()
    {
        return $"$r{_recordIndex++}";
    }
    
    public override object? VisitVariable_declaration(AmethystParser.Variable_declarationContext context)
    {
        object? result = null;
        Type? type = null;
        
        if (context.expression() is { } expression)
        {
            result = VisitExpression(expression);
        }
        
        if (context.type() is { } typeContext)
        {
            type = VisitType(typeContext);
        }
        else if (result == null)
        {
            ThrowSyntaxError("Variable declarations must have specified a type or an expression to infer the type from.", context); 
        }
        
        if (InferType(result, out var inferredType))
        {
            if (type is not null && !type.Equals(inferredType))
            {
                ThrowSyntaxError($"The type of the variable '{type}' does not match the inferred type '{inferredType}'.", context);
            }
            
            type = inferredType;
        }
        else if (type is null)
        {
            ThrowSyntaxError("Could not infer the type of the variable.", context);
        }

        var name = GetVariableName();
        
        AddInitCode($"scoreboard players reset {name} amethyst");

        return null;
    }
}