using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    private string GetVariableName()
    {
        return Scope.VariableCount.ToString();
    }
    
    public override object? VisitVariable_declaration(AmethystParser.Variable_declarationContext context)
    {
        var variableName = context.identifier().GetText();
        
        var name = GetVariableName();
        
        object? result = null;
        Type? type = null;
        
        if (context.expression() is { } expression)
        {
            result = VisitExpression(expression); // Todo: Destination targeting (tell VisitExpression that the result should be stored in 'name')
        }
        
        if (context.type() is { } typeContext)
        {
            type = VisitType(typeContext);
        }
        else if (result == null)
        {
            ThrowSyntaxError("Variable declarations must have specified a type or an expression to infer the type from.", context); 
        }
        
        if (/*InferType(result, out var inferredType)*/ false) // result.Type
        {
            // if (type is not null && !type.Equals(inferredType))
            // {
            //     ThrowSyntaxError($"The type of the variable '{type}' does not match the inferred type '{inferredType}'.", context);
            // }
            //
            // type = inferredType;
        }
        else if (type is null)
        {
            ThrowSyntaxError("Could not infer the type of the variable.", context);
        }
        
        if (Scope.Variables.ContainsKey(variableName))
        {
            ThrowSyntaxError($"The variable '{variableName}' has already been declared.", context);
        }

        Scope.Variables.Add(variableName, new Variable
        {
            Name = name,
            Type = type!
        });

        if (Program.DebugMode && type!.IsScoreboardType && result != null)
        {
            AddInitCode($$"""
                           scoreboard players display name {{name}} amethyst ["",{"text":"{{name}}: ","color":"dark_purple"},{"text":"{{Scope.McFunctionPath}}/","color":"gray"},{"text":"{{variableName}}","color":"light_purple"}]
                           """);
        }

        return null;
    }
}