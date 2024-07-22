using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    private string GetVariableName()
    {
        if (Program.DebugMode)
        {
            return (TotalVariableCount-1).ToString();
        }
        
        return Scope.VariableCount.ToString();
    }
    
    public override object? VisitVariable_declaration(AmethystParser.Variable_declarationContext context)
    {
        TotalVariableCount++;
        
        var variableName = context.identifier().GetText();
        
        if (Scope.Variables.ContainsKey(variableName))
        {
            throw new SyntaxException($"The variable '{variableName}' has already been declared.", context);
        }
        
        var name = GetVariableName();
        
        Result? result = null;
        Type? type = null;
        
        if (context.expression() is { } expression)
        {
            result = VisitExpressionTargeted(expression, target: name);
        }
        
        // if a type is defined, set the type to the defined type
        if (context.type() is { } typeContext)
        {
            type = VisitType(typeContext);
        }
        // if both types are defined, check if they match
        if (type != null && result != null && type != result.Type)
        {
            throw new SyntaxException($"The type of the variable '{type}' does not match the inferred type '{result.Type}'.", context);
        }
        // if no type is defined or inferred, throw an error
        if (type == null && result == null)
        {
            throw new SyntaxException("Variable declarations must have specified a type or an expression to infer the type from.", context);
        }
        // if no type is defined, but inferred, set the type to the inferred type
        if (type == null && result != null)
        {
            type = result.Type;
        }
        
        Debug.Assert(type != null);

        Scope.Variables.Add(variableName, new Variable
        {
            Name = name,
            Type = type
        });

        if (type.IsScoreboardType)
        {
            if (result == null)
            {
                AddCode($"scoreboard players reset {name} amethyst");
            }
            
            if (Program.DebugMode)
            {
                AddInitCode($$"""scoreboard players display name {{name}} amethyst ["",{"text":"{{name}}: ","color":"dark_purple"},{"text":"{{Scope.McFunctionPath}}/","color":"gray"},{"text":"{{variableName}}","color":"light_purple"}]""");
            }
        }
        else if (type.IsStorageType)
        {
            if (Program.DebugMode)
            {
                name = $"{Scope.McFunctionPath}/{variableName}"; // Todo: Move this code to the destination targeting, and the result simply contains this name in "string Location"
            }
            
            if (result == null)
            {
                AddCode($"data remove storage amethyst:variables {name}");
            }
        }

        return null;
    }
}