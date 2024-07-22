using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    private string GetRecordName()
    {
        if (Program.DebugMode)
        {
            return $"amethyst_r{TotalRecordCount-1}";
        }
        
        return $"amethyst_r{Scope.RecordCount}";
    }
    
    public override object? VisitRecord_declaration(AmethystParser.Record_declarationContext context)
    {
        TotalRecordCount++;
        
        var recordName = context.identifier().GetText();
        
        if (Scope.Records.ContainsKey(recordName))
        {
            throw new SyntaxException($"The record '{recordName}' has already been declared.", context);
        }
        
        var name = GetRecordName();
        
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
            throw new SyntaxException($"The type of the record '{type}' does not match the inferred type '{result.Type}'.", context);
        }
        // if no type is defined or inferred, throw an error
        if (type == null && result == null)
        {
            throw new SyntaxException("Record declarations must have specified a type or an expression to infer the type from.", context);
        }
        // if no type is defined, but inferred, set the type to the inferred type
        if (type == null && result != null)
        {
            type = result.Type;
        }
        
        Debug.Assert(type != null);

        Scope.Records.Add(recordName, new Record
        {
            Name = name,
            Type = type,
            InitialValue = result,
            Attributes = VisitAttribute_list(context.attribute_list())
        });

        if (type.IsScoreboardType)
        {
            AddInitCode($"scoreboard objectives add {name} dummy");
            AddInitCode($"scoreboard players reset * {name}");
            
            // The actual values are being set when they are needed, so we don't need to set them here

            if (Program.DebugMode)
            {
                AddInitCode($$"""scoreboard objectives modify {{name}} displayname ["",{"text":"Record ","bold":true},{"text":"{{name}}","color":"dark_purple"},{"text":" @ "},{"text":"{{Scope.McFunctionPath}}/","color":"gray"},{"text":"{{recordName}}","color":"light_purple"}]""");
            }
        }
        else if (type.IsStorageType)
        {
            if (Program.DebugMode)
            {
                name = $"{Scope.McFunctionPath}/{recordName}"; // Todo: Move this code to the destination targeting, and the result simply contains this name in "string Location"
            }
            
            if (result == null)
            {
                AddCode($"data remove storage amethyst:records {name}");
            }
        }

        return null;
    }
}